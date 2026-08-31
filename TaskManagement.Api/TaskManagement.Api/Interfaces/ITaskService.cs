using TaskManagement.Api.DTOS;
using TaskManagement.Api.Responses;

namespace TaskManagement.Api.Interfaces
{
    public interface ITaskService
    {
        PagedResponse<TaskDto> GetAll(TaskQuery query);
        TaskDto? GetById(int id);
        Task<TaskDto> CreateAsync(CreateTaskDto createTaskDto, CancellationToken cancellationToken = default);
        Task<TaskDto?> UpdateAsync(int id, UpdateTaskDto updateTaskDto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id,CancellationToken cancellationToken = default);
    }
}
/*
 Burada ITaskService şunu söylüyor: 

 Yani kim ITaskService kullanıyorsa bu 5 metodu sağlamak zorunda.

 */