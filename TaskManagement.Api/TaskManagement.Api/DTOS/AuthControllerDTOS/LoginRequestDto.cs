namespace TaskManagement.Api.DTOS.AuthControllerDTOS
{
    public record LoginRequestDto
    {
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}
