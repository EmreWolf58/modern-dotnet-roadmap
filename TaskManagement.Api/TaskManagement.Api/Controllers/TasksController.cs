using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.Model.TaskModel;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        //geçici örnek model verisi
        private static List<TaskModel> tasks = new List<TaskModel>
        {
            new TaskModel
            {
                Id = 1,
                Title = "ASP.NET Core öğren",
                Description = "Controller ve endpoint mantığını öğren",
                IsCompleted = false,
                CreatedDate = DateTime.Now
            },
            new TaskModel
            {
                Id = 2,
                Title = "Swagger testleri yap",
                Description = "GET, POST, PUT, DELETE endpointlerini dene",
                IsCompleted = false,
                CreatedDate = DateTime.Now
            }
        };

        [HttpGet]
        public ActionResult<List<TaskModel>> GetAll()
        {
            return Ok(tasks);
        }
        [HttpGet("{id}")]
        public ActionResult<List<TaskModel>> GetById(int id)
        {
            var tasksRet = tasks.FirstOrDefault(x => x.Id == id);
            
            if (tasksRet == null)
            {
                return NotFound();
            }

            return Ok(tasksRet);
        }
        [HttpPost]
        public ActionResult<TaskModel> Create(TaskModel model)
        {
            model.Id = tasks.Max(x => x.Id) + 1;
            model.CreatedDate = DateTime.Now;
            tasks.Add(model);

            return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id , TaskModel model)
        {
            var tasksRet = tasks.FirstOrDefault(x => x.Id == id);

            if(tasksRet == null)
            {
                return NotFound();
            }

            tasksRet.Title = model.Title;
            tasksRet.Description = model.Description;
            tasksRet.IsCompleted = model.IsCompleted;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var task = tasks.FirstOrDefault(x => x.Id == id);

            if (task == null)
                return NotFound();

            tasks.Remove(task);

            return NoContent();
        }
    }
}
