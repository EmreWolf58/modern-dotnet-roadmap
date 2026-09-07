using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.Interfaces;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttachmentsController: ControllerBase
    {
        private readonly IFileService _fileService;
        public AttachmentsController(IFileService fileService)
        {
            _fileService= fileService;
        }

        [HttpPost("upload")] //POST /api/attachments/upload bunu oluşturur.
        public async Task<IActionResult> Upload(IFormFile file) //IFormFile file ise HTTP isteğinden gelen dosyayı ASP.NET Core'un bizim için almasını sağlar.
        {
            var result = await _fileService.UploadAsync(file);

            return Ok(result);
        }

        [HttpPost("upload-multiple")]
        public async Task<IActionResult> UploadMultiple (List<IFormFile> files)
        {
            var result = await _fileService.UploadMultipleAsync(files);
            return Ok(result);
        }

        [HttpGet("download/{fileName}")]
        public IActionResult Download(string fileName)
        {
            var result = _fileService.Download(fileName);

            if (result == null)
            {
                return NotFound();
            }

            return File(result.Stream, result.ContentType, result.FileName);
        }
    }
}
