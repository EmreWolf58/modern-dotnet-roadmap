using TaskManagement.Api.DTOS;

namespace TaskManagement.Api.Interfaces
{
    public interface IExternalTodoService
    {
        Task<ExternalTodoDto?> GetTodoAsync(int id, CancellationToken cancellationToken);
    }
}
