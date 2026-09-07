using TaskManagement.Api.DTOS;

namespace TaskManagement.Api.Interfaces
{
    public interface IFileService
    {
        Task<FileUploadResultDto> UploadAsync(IFormFile file);
        Task<List<FileUploadResultDto>> UploadMultipleAsync(List<IFormFile> files);
        FileDownloadDto? Download (string fileName);
    }
}
/*
 Burada controller artık şunu bilecek:

        Dosya yüklemek istiyorsam → UploadAsync

        Çoklu yüklemek istiyorsam → UploadMultipleAsync

        İndirmek istiyorsam → Download

Nasıl yapıldığıyla ilgilenmeyecek.
 */