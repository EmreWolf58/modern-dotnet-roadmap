using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    //[Route("api/v{version:apiVersion}/tasks")] // URL Versioning için bu yapıyı kullanıyoruz. Bu sayede URL üzerinden versiyonlama yapabiliyoruz.
    [Route("api/version-test/tasks")]
    public class TasksController: ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Version = "v1",
                Message = "Task Api Version 1"
            });
        }
    }
}
