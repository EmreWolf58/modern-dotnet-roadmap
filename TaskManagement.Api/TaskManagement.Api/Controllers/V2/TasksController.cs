using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.Api.Controllers.V2
{
    [ApiController]
    [ApiVersion("2.0")]
    //[Route("api/v{version:apiVersion}/tasks")]  // URL Versioning için bu yapıyı kullanıyoruz. Bu sayede URL üzerinden versiyonlama yapabiliyoruz.
    [Route("api/version-test/tasks")]
    public class TasksController: ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Version = "v2",
                Message = "Task API version 2",
                NewFeature = "Bu alan sadece v2 de bulunuyor. Yeni versiyonda eklendi."
            });
        }
    }
}
