namespace TaskManagement.Api.DTOS
{
    public class FileUploadResultDto
    {
        public string OrginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public long Length { get; set; }
        public string ContentType { get; set; } = string.Empty;
    }
}
