namespace TaskManagement.Api.Responses
{
    public class ApiErrorResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
        public string? Path { get; set; }
        public string? TraceId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
