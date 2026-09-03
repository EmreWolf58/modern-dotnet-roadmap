using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttachmentsController: ControllerBase
    {
        private const long MaxFileSize = 5 * 1024 * 1024;
        private static readonly string[] AllowedExtensions =
        {
            ".pdf",
            ".jpg",
            ".jpeg",
            ".png"
        };
        private static readonly string[] AllowedContentTypes =
        {
            "application/pdf",
            "image/jpeg",
            "image/png"
        };

        [HttpPost("upload")] //POST /api/attachments/upload bunu oluşturur.
        public async Task<IActionResult> Upload(IFormFile file) //IFormFile file ise HTTP isteğinden gelen dosyayı ASP.NET Core'un bizim için almasını sağlar.
        {
            var extension = Path.GetExtension(file.FileName);

            if (file.Length > MaxFileSize)
            {
                return BadRequest("Dosya boyutu en fazla 5 MB olabilir.");
            }
            if (!AllowedExtensions.Contains(extension.ToLowerInvariant()))
            {
                return BadRequest("Desteklenmeyen dosya türü.");
            }
            if (file == null || file.Length == 0) // dosyagelmediyse ve dosya geldi ama boşsa yı kontrol ediyoruz.
            {
                return BadRequest("Dosya gönderilemedi.");
            }
            if (!AllowedContentTypes.Contains(file.ContentType))
            {
                return BadRequest("Desteklenmeyen dosya içerik türü.");
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads"); //Directory.GetCurrentDirectory() uygulamanın çalıştığı ana klasörü verir.


            if (!Directory.Exists(uploadsFolder))//Directory.Exists ile klasör var mı bakıyoruz.
            {
                Directory.CreateDirectory(uploadsFolder); //yoksa Directory.CreateDirectory ile klasörü oluşturuyoruz
            }

            var safeFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, safeFileName); //dosyanın tam yolunu oluşturuyoruz.
            using var stream = new FileStream(filePath, FileMode.Create); //dosya üzerindeveri okuyup yazmamızı sağlayan stream dir.
            //FileMode.Create Bu path'te dosya oluştur demektir.
            await file.CopyToAsync(stream); // dosyayı kaydeden satır.

            return Ok(new
            {
                file.FileName,
                file.Length,
                file.ContentType
            });
        }
    }
}
