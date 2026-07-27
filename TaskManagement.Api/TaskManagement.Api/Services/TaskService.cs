using Microsoft.AspNetCore.Mvc.ViewFeatures;
using TaskManagement.Api.DTOS;
using TaskManagement.Api.Model.TaskModel;
using TaskManagement.Api.Interfaces;
using Microsoft.Extensions.Options;
using TaskManagement.Api.Settings;
using AutoMapper;
using TaskManagement.Api.Events;

namespace TaskManagement.Api.Services
{
    public class TaskService: ITaskService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationSettings _settings;
        private readonly TaskEventPublisher _taskEventPublisher;
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

        public TaskService(IOptions<ApplicationSettings> settings, IMapper mapper, TaskEventPublisher taskEventPublisher)
        {
            _settings = settings.Value;
            _mapper = mapper;
            _taskEventPublisher = taskEventPublisher;

        }

        public List<TaskDto> GetAll()
        {
            return _mapper.Map<List<TaskDto>>(_tasks);
        }

        public TaskDto? GetById (int id)
        {
            var task = _tasks.FirstOrDefault(task => task.Id == id);

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
            var task = _tasks.FirstOrDefault(x => x.Id == id);

            if (task is null)
                return null;

            _mapper.Map(updateTaskDto, task);

            return _mapper.Map<TaskDto>(task);
        }

        public bool Delete (int id)
        {
            var task = _tasks.FirstOrDefault(x => x.Id == id);

            if (task is null)
                return false;

            _tasks.Remove(task);

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
