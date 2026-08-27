using Microsoft.AspNetCore.Mvc.ViewFeatures;
using TaskManagement.Api.DTOS;
using TaskManagement.Api.Model.TaskModel;
using TaskManagement.Api.Interfaces;
using Microsoft.Extensions.Options;
using TaskManagement.Api.Settings;
using AutoMapper;
using TaskManagement.Api.Events;
using TaskManagement.Api.Responses;
using Microsoft.Extensions.Caching.Memory;

namespace TaskManagement.Api.Services
{
    public class TaskService: ITaskService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationSettings _settings;
        private readonly TaskEventPublisher _taskEventPublisher;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger _logger;
        private readonly List<TaskModel> _tasks = new()
        {
            new TaskModel
            {
                Id = 1,
                Title = "ASP.NET Core öğren",
                Description = "Controller ve routing konularını tekrar et.",
                IsCompleted = false,
                CreatedDate = DateTime.Now
            },
            new TaskModel
            {
                Id = 2,
                Title = "CRUD endpointlerini yaz",
                Description = "GET, POST, PUT, DELETE endpointlerini tamamla.",
                IsCompleted = true,
                CreatedDate = DateTime.Now
            }
        };

        public TaskService(IOptions<ApplicationSettings> settings, IMapper mapper, TaskEventPublisher taskEventPublisher, IMemoryCache memoryCache, ILogger<TaskService> logger)
        {
            _settings = settings.Value;
            _mapper = mapper;
            _taskEventPublisher = taskEventPublisher;
            _memoryCache = memoryCache;
            _logger = logger;

        }

        public PagedResponse<TaskDto> GetAll( TaskQuery query)
        {
            var cacheKey = $"tasks_{query.Search}_{query.Completed}_{query.SortBy}_{query.Descending}_{query.Page}_{query.PageSize}_{query.IncludeDeleted}";

            if (_memoryCache.TryGetValue(cacheKey, out PagedResponse<TaskDto>? cachedResponse)) //cacheye bakıyo varsa yolluyor hiç alta bakmadan bu bir cache hit.
            {
                _logger.LogInformation("CACHE HIT: {CacheKey}", cacheKey);
                return cachedResponse!;
            }

            IEnumerable<TaskModel> filteredTasks = _tasks;

            if (!query.IncludeDeleted)
            {
                filteredTasks = filteredTasks.Where(task => !task.IsDeleted);
            }

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                filteredTasks = filteredTasks.Where(task =>
                    task.Title.Contains(
                        query.Search,
                        StringComparison.OrdinalIgnoreCase
                    ) ||
                    task.Description.Contains(
                        query.Search,
                        StringComparison.OrdinalIgnoreCase
                    )
                );
            }

            if (query.Completed.HasValue)
            {
                filteredTasks = filteredTasks.Where(task =>
                    task.IsCompleted == query.Completed.Value
                );
            }

            filteredTasks = query.SortBy.ToLowerInvariant() switch
            {
                "title" => query.Descending
                    ? filteredTasks.OrderByDescending(task => task.Title)
                    : filteredTasks.OrderBy(task => task.Title),

                "createddate" => query.Descending
                    ? filteredTasks.OrderByDescending(task => task.CreatedDate)
                    : filteredTasks.OrderBy(task => task.CreatedDate),

                "id" => query.Descending
                    ? filteredTasks.OrderByDescending(task => task.Id)
                    : filteredTasks.OrderBy(task => task.Id),

                _ => filteredTasks.OrderBy(task => task.Id)
            };

            var totalCount = filteredTasks.Count();

            var pagedTasks = filteredTasks
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            var taskDtos = _mapper.Map<List<TaskDto>>(pagedTasks);

            var totalPages = (int)Math.Ceiling(
                totalCount / (double)query.PageSize
            );

            var response = new PagedResponse<TaskDto>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = taskDtos
            };
            _logger.LogInformation("CACHE MISS: {CacheKey}", cacheKey);
            //var cacheOptions = new MemoryCacheEntryOptions // cacheye bir süre ekledik. Absolute Expiration 30 saniye cachede tut demek
            //{
            //    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
            //};

            //var cacheOptions = new MemoryCacheEntryOptions
            //{
            //    SlidingExpiration= TimeSpan.FromSeconds(10) // sliding expiration cachedeki itemin son erişiminden itibaren 30 saniye boyunca cachede kalmasını sağlar.
            //                                                // 30 saniye boyunca erişilmezse cacheden silinir.
            //                                                //eğer erişilirse 10 saniye devam eder.
            //};

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30),
                SlidingExpiration = TimeSpan.FromSeconds(10)
                /*
                Bu durumda iki kuralımız var:
                Absolute → maksimum 30 saniye
                Sliding → 10 saniye hiç kullanılmazsa sil
                Hangisi önce gerçekleşirse cache expire olur.
                */
            };
            _memoryCache.Set(cacheKey, response, cacheOptions); // süresiz cacheye ekler (cacheOptions olmadan varsa değişebilir durum.)

            

            return response;
        }

        public TaskDto? GetById (int id)
        {
            var task = _tasks.FirstOrDefault(task => task.Id == id && !task.IsDeleted);

            if (task is null)
            {
                return null;
            }

            return _mapper.Map<TaskDto>(task);
        }

        public TaskDto Create(CreateTaskDto createTaskDto)
        {
            var task = _mapper.Map<TaskModel>(createTaskDto);

            task.Id = _tasks.Any() ? _tasks.Max(x => x.Id) + 1 : 1;

            task.IsCompleted = false;
            task.CreatedDate = DateTime.Now;

            _tasks.Add(task);

            _taskEventPublisher.PublishTaskCreated(task.Title);

            return _mapper.Map<TaskDto>(task);
        }

        public TaskDto? Update(int id, UpdateTaskDto updateTaskDto)
        {
            var task = _tasks.FirstOrDefault(x => x.Id == id && !x.IsDeleted);

            if (task is null)
                return null;

            _mapper.Map(updateTaskDto, task);

            return _mapper.Map<TaskDto>(task);
        }

        public bool Delete (int id)
        {
            var task = _tasks.FirstOrDefault(x => x.Id == id && !x.IsDeleted);

            if (task is null)
                return false;

            task.IsDeleted = true;
            task.DeletedDate = DateTime.Now;
            

            return true;
        }

        public static string GetTaskStatus(TaskModel task)
        {
            return task switch
            {
                {IsCompleted: true } => "Tamamlandı", //{IsCompleted: true } burası property pattern örneğidir.TaskModel nesnesinin IsCompleted değeri true ise anlamına gelir.
                { IsCompleted:false } => "Devam Ediyor"
            };
        }

        private static string GetTaskCountStatus(int taskCount) //Relational Pattern örneği
        {
            return taskCount switch
            {
                0 => "Hiç task yok",
                1 => "Bir task var",
                >= 2 and <= 5 => "Az sayıda task var",
                > 5 => "Çok sayıda task var",
                _ => "Geçersiz değer"
            };
        }
    }
}
