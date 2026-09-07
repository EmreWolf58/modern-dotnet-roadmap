using TaskManagement.Api.DTOS;
using TaskManagement.Api.Interfaces;

namespace TaskManagement.Api.Services
{
    public class FileService: IFileService
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

        private readonly string _uploadsFolder;

        public FileService(IWebHostEnvironment environment)
        {
            _uploadsFolder = Path.Combine(environment.ContentRootPath, "Uploads");
            Directory.CreateDirectory(_uploadsFolder); //if içinde yazmaya gerek yok zaten varsa klasör hata vermiyor.
        }

        public async Task<FileUploadResultDto> UploadAsync (IFormFile file)
        {
            ValidateFile(file);
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var safeFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_uploadsFolder, safeFileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return new FileUploadResultDto
            {
                OrginalFileName = file.FileName,
                StoredFileName = safeFileName,
                Length = file.Length,
                ContentType = file.ContentType
            };
        }

        public async Task<List<FileUploadResultDto>> UploadMultipleAsync(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                throw new ArgumentException("Dosya gönderilemedi.");
            }

            var results = new List<FileUploadResultDto>();

            foreach (var file in files)
            {
                var result = await UploadAsync(file);
                results.Add(result);
            }

            return results;
        }

        public FileDownloadDto? Download(string fileName)
        {
            var SafeFileName = Path.GetFileName(fileName);

            if (SafeFileName != fileName)
            {
                return null;
            }

            var filePath = Path.Combine(_uploadsFolder, SafeFileName);

            if (!File.Exists(filePath))
            {
                return null;
            }

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var contentType = GetContentType(SafeFileName);

            return new FileDownloadDto
            {
                Stream = stream,
                ContentType = contentType,
                FileName = SafeFileName
            };
        }

        private void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Dosya Gönderilemedi");
            }

            if (file.Length > MaxFileSize)
            {
                throw new ArgumentException("Dosya boyutu en fazla 5 MB olabilir.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Desteklenmeyen dosya uzantısı.");
            }

            if (!AllowedContentTypes.Contains(file.ContentType))
            {
                throw new ArgumentException("Desteklenmeyen dosya içerik türü.");
            }
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
