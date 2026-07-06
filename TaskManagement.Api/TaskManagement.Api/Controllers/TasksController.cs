using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.DTOS;
using TaskManagement.Api.Model.TaskModel;
using TaskManagement.Api.Services;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TaskService _taskServices;

        public TasksController(TaskService taskService)
        {
            _taskServices = taskService;
        }
        

        [HttpGet]
        public ActionResult<List<TaskDto>> GetAll()
        {
            var tasks = _taskServices.GetAll();

            return Ok(tasks);
        }
        [HttpGet("{id}")]
        public ActionResult<List<TaskDto>> GetById(int id)
        {
            var task = _taskServices.GetById(id);

            if (task == null)
            {
                return NotFound("Task bulunamadı.");
            }
            
            return Ok(task);
        }
        [HttpPost]
        public ActionResult<TaskDto> Create(CreateTaskDto createTaskDto)
        {
            if (string.IsNullOrWhiteSpace(createTaskDto.Title))
            {
                return BadRequest("Title alanı boş olamaz.");
            }

            var createdTask = _taskServices.Create(createTaskDto);

            return CreatedAtAction(nameof(GetById), new { id = createdTask.Id }, createdTask);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id , UpdateTaskDto model)
        {
            var result = _taskServices.Update(id, model);

            if (!result)
            {
                return NotFound("Güncellenicek task bulunamadı.");
            }

            return NoContent(); //204 e karşılık geliyor
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _taskServices.Delete(id);

            if (!result)
            {
                return NotFound("Silinecek task bulunamadı.");
            }

            return NoContent(); //204 e karşılık geliyor.
        }
    }
}
