using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.DTOS;
using TaskManagement.Api.Interfaces;
using TaskManagement.Api.Responses;

namespace TaskManagement.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskServices;
        private readonly ILogger<TasksController> _logger; //Bu Controller içinde log yazmak için bir logger kullanacağım.

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskServices = taskService;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<ApiResponse<PagedResponse<TaskDto>>> GetAll([FromQuery] TaskQuery query)
        {
            _logger.LogInformation(
        "Task listesi istendi. Page: {Page}, PageSize: {PageSize}, " +
        "Search: {Search}, Completed: {Completed}, SortBy: {SortBy}, " +
        "Descending: {Descending}, IncludeDeleted: {IncludeDeleted}",
        query.Page,
        query.PageSize,
        query.Search,
        query.Completed,
        query.SortBy,
        query.Descending,
        query.IncludeDeleted
    );

            var result = _taskServices.GetAll(query);

            _logger.LogInformation(
        "{ListedTaskCount} adet task listelendi. " +
        "Filtreye uyan toplam task: {TotalCount}",
        result.Items.Count,
        result.TotalCount);

            var response = ApiResponse<PagedResponse<TaskDto>>.CreateSuccess(result, "Task listesi başarıyla getirildi.");

            return Ok(response);
        }

        [HttpGet("{id}")]
        public ActionResult<ApiResponse<List<TaskDto>>> GetById(int id)
        {
            _logger.LogInformation("Task detayı istendi. TaskId: {TaskId}", id);
            var task = _taskServices.GetById(id);

            if (task is null)
            {
                _logger.LogInformation("Task bulunamadı. TaskId: {TaskId}", id);
                var errorResponse = ApiResponse<TaskDto>.CreateFailure("Task bulunamadı.");
                return NotFound(errorResponse);
            }
            _logger.LogInformation("Task bulundu. TaskId: {TaskId}", id);
            var response = ApiResponse<TaskDto>.CreateSuccess(task, "Task başarıyla getirildi.");
            return Ok(response);
        }

        [HttpPost]
        public ActionResult<ApiResponse<TaskDto>> Create(CreateTaskDto createTaskDto)
        {
            _logger.LogInformation("Yeni task oluşturma isteği geldi. Title: {Title}", createTaskDto.Title);

            var createdTask = _taskServices.Create(createTaskDto);

            _logger.LogInformation("Yeni task oluşturuldu. TaskId: {TaskId}", createdTask.Id);

            var response = ApiResponse<TaskDto>.CreateSuccess(createdTask, "Task başarıyla oluşturuldu.");


            return CreatedAtAction(nameof(GetById), new { id = createdTask.Id }, response);
        }

        [HttpPut("{id}")]
        public ActionResult<ApiResponse<TaskDto>> Update(int id, UpdateTaskDto model)
        {
            _logger.LogInformation("Task güncelleme isteği geldi. TaskId: {TaskId}", id);

            var result = _taskServices.Update(id, model);

            if (result is null)
            {
                _logger.LogInformation("Task güncellenemedi çünkü bulunamadı. TaskId: {TaskId}", id);
                return NotFound("Güncellenicek task bulunamadı.");
            }
            _logger.LogInformation("Task Güncellendi. TaskId: {TaskId}", id);
            var response = ApiResponse<TaskDto>.CreateSuccess(result, "Task başarıyla güncellendi.");

            return Ok(response); //204 e karşılık geliyor
        }

        [HttpDelete("{id}")]
        public ActionResult<ApiResponse<object>> Delete(int id)
        {
            _logger.LogInformation("Task silme isteği geldi. TaskId: {TaskId}", id);
            var result = _taskServices.Delete(id);

            if (!result)
            {
                _logger.LogInformation("Task silinemedi çünkü bulunamadı. TaskId: {TaskId}", id);
                var errorResponse = ApiResponse<object>.CreateFailure("Silinecek task bulunamadı.");
                return NotFound(errorResponse);
            }
            _logger.LogInformation("Task silindi. TaskId: {TaskId}", id);
            var response = ApiResponse<object>.CreateSuccess(null, "Task başarıyla silindi.");
            return Ok(response);
        }
    }
}
