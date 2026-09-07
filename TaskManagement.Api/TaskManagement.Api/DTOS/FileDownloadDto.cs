namespace TaskManagement.Api.DTOS
{
    public class FileDownloadDto
    {
        public Stream Stream { get; set; } = Stream.Null;
        public string ContentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;

    }
}
