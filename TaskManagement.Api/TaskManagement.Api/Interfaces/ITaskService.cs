using TaskManagement.Api.DTOS;

namespace TaskManagement.Api.Interfaces
{
    public interface ITaskService
    {
        List<TaskDto> GetAll();
        TaskDto? GetById(int id);
        TaskDto Create(CreateTaskDto createdTaskDto);
        bool Update(int id, UpdateTaskDto updateTaskDto);
        bool Delete(int id);
    }
}
/*
 Burada ITaskService şunu söylüyor:

 Yani kim ITaskService kullanıyorsa bu 5 metodu sağlamak zorunda.

 */