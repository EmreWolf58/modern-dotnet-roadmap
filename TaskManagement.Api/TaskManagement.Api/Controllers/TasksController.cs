using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.DTOS;
using TaskManagement.Api.Interfaces;
using TaskManagement.Api.Model.TaskModel;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskServices;
        private ILogger<TasksController> _logger; //Bu Controller içinde log yazmak için bir logger kullanacağım.

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskServices = taskService;
            _logger = logger;
        }
        

        [HttpGet]
        public ActionResult<List<TaskDto>> GetAll()
        {
            _logger.LogInformation("Task listesi istendi");
            var tasks = _taskServices.GetAll();
            _logger.LogInformation("{TaskCount} adet task listelendi", tasks.Count);
            return Ok(tasks);
        }
        [HttpGet("{id}")]
        public ActionResult<List<TaskDto>> GetById(int id)
        {
            _logger.LogInformation("Task detayı istendi. TaskId: {TaskId}", id);
            var task = _taskServices.GetById(id);

            if (task == null)
            {
                _logger.LogInformation("Task bulunamadı. TaskId: {TaskId}", id);
                return NotFound("Task bulunamadı.");
            }
            _logger.LogInformation("Task bulundu. TaskId: {TaskId}",id);
            return Ok(task);
        }
        [HttpPost]
        public ActionResult<TaskDto> Create(CreateTaskDto createTaskDto)
        {
            _logger.LogInformation("Yeni task oluşturma isteği geldi. Title: {Title}", createTaskDto.Title);
            if (string.IsNullOrWhiteSpace(createTaskDto.Title))
            {
                _logger.LogInformation("Task oluşturulamadı. Title boş gönderildi.");
                return BadRequest("Title alanı boş olamaz.");
            }

            var createdTask = _taskServices.Create(createTaskDto);

            _logger.LogInformation("Yeni task oluşturuldu. TaskId: {TaskId}", createdTask.Id);
            return CreatedAtAction(nameof(GetById), new { id = createdTask.Id }, createdTask);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id , UpdateTaskDto model)
        {
            _logger.LogInformation("Task güncelleme isteği geldi. TaskId: {TaskId}",id);
            var result = _taskServices.Update(id, model);

            if (!result)
            {
                _logger.LogInformation("Task güncellenemedi çünkü bulunamadı. TaskId: {TaskId}", id);
                return NotFound("Güncellenicek task bulunamadı.");
            }
            _logger.LogInformation("Task Güncellendi. TaskId: {TaskId}",id);
            return NoContent(); //204 e karşılık geliyor
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation("Task silme isteği geldi. TaskId: {TaskId}",id);
            var result = _taskServices.Delete(id);

            if (!result)
            {
                _logger.LogInformation("Task silinemedi çünkü bulunamadı. TaskId: {TaskId}", id);
                return NotFound("Silinecek task bulunamadı.");
            }
            _logger.LogInformation("Task silindi. TaskId: {TaskId}", id);
            return NoContent(); //204 e karşılık geliyor.
        }
    }
}
