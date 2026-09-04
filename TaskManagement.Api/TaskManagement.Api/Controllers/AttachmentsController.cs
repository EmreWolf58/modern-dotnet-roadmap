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

        [HttpPost("upload-multiple")]
        public async Task<IActionResult> UploadMultiple (List<IFormFile> files)
        {
            if (files == null || files.Count ==0)
            {
                return BadRequest("Dosya gönderilemedi.");
            }

            var uploadedFiles = new List<object>();

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            foreach (var file in files)
            {
                if (file.Length == 0)
                {
                    continue; // boş dosyaları atla
                }

                if (file.Length > MaxFileSize)
                {
                    return BadRequest($"{file.FileName} dosyası 5 MB'dan büyük.");
                }

                var extension = Path.GetExtension(file.FileName);

                if (!AllowedExtensions.Contains(extension.ToLowerInvariant()))
                {
                    return BadRequest($"{file.FileName} desteklenmeyen uzantıya sahip.");
                }

                if (!AllowedContentTypes.Contains(file.ContentType))
                {
                    return BadRequest($"{file.FileName} desteklenmeyen içerik türüne sahip.");
                }

                var safeFileName = $"{Guid.NewGuid()}{extension.ToLowerInvariant()}";
                var filePath = Path.Combine(uploadsFolder,safeFileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                uploadedFiles.Add(new
                {
                    OriginalFileName = file.FileName,
                    StoredFileName = safeFileName,
                    file.Length,
                    file.ContentType
                });
            }
            return Ok(uploadedFiles);
        }

        [HttpGet("download/{fileName}")]
        public IActionResult Download(string fileName)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            var filePath = Path.Combine(uploadsFolder, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Dosya bulunamadı.");
            }

            //var fileBytes = System.IO.File.ReadAllBytes(filePath); // ReadAllBytes dosyanın tamamını ram e aldığı için problemli
            //var contentType = GetContentType(fileName);
            //return File(fileBytes, contentType, fileName);

            var stream = new FileStream(filePath,FileMode.Open, FileAccess.Read);
            var contentType = GetContentType(fileName);
            return File(stream, contentType, fileName);
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
        }
    }
}
