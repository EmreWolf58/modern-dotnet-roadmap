using Microsoft.AspNetCore.Mvc.ViewFeatures;
using TaskManagement.Api.DTOS;
using TaskManagement.Api.Model.TaskModel;
using TaskManagement.Api.Interfaces;
using Microsoft.Extensions.Options;
using TaskManagement.Api.Settings;

namespace TaskManagement.Api.Services
{
    public class TaskService: ITaskService
    {
        private readonly ApplicationSettings _settings;
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

        public TaskService(IOptions<ApplicationSettings> settings)
        {
            _settings = settings.Value;

            _tasks = new List<TaskModel>()
            {

            };
        }

        private TaskDto MapToDto(TaskModel task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                CreatedDate = task.CreatedDate
            };
        }

        public List<TaskDto> GetAll()
        {
            return _tasks.Select(task => MapToDto(task)).ToList();
        }

        public TaskDto? GetById (int id)
        {
            var task = _tasks.FirstOrDefault(task => task.Id == id);

            if (task == null)
            {
                return null;
            }

            return MapToDto(task);
        }

        public TaskDto Create(CreateTaskDto createTaskDto)
        {
            if (_tasks.Count >= _settings.MaxTaskCount)
            {
                throw new Exception("Task limiti doldu.");
            }

            var newTask = new TaskModel
            {
                Id = _tasks.Any() ? _tasks.Max(x => x.Id) + 1 : 1,
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                IsCompleted = false,
                CreatedDate = DateTime.Now
            };
            _tasks.Add(newTask);

            return MapToDto(newTask);
        }

        public bool Update(int id, UpdateTaskDto updateTaskDto)
        {
            var task = _tasks.FirstOrDefault(x => x.Id == id);

            if (task == null)
            {
                return false;
            }

            task.Title = updateTaskDto.Title;
            task.Description = updateTaskDto.Description;
            task.IsCompleted = updateTaskDto.IsCompleted;

            return true;
        }

        public bool Delete (int id)
        {
            var task = _tasks.FirstOrDefault(x => x.Id == id);

            if (task == null)
                return false;

            _tasks.Remove(task);

            return true;
        }
    }
}
