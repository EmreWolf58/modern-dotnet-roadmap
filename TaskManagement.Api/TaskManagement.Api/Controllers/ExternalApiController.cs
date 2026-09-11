using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using TaskManagement.Api.DTOS;
using TaskManagement.Api.Interfaces;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalApiController : ControllerBase
    {
        private readonly IExternalTodoService _externalTodoService;

        public ExternalApiController(IExternalTodoService externalTodoService)
        {
            _externalTodoService = externalTodoService;
        }

        [HttpGet("todos/{id}")]
        public async Task<IActionResult> GetTodo(int id, CancellationToken cancellationToken)
        {
            var todo = await _externalTodoService.GetTodoAsync(id, cancellationToken);
            return Ok(todo);
        }
    }
}
/*
 HttpResponseMessage

Harici API'nin verdiği HTTP cevabını temsil eder.
İçinde şunlar vardır:

StatusCode
Headers
Content
IsSuccessStatusCode
 */
