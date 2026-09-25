using TaskManagement.Api.Model.TaskModels;

namespace TaskManagement.Api.Model
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<TaskModel> Tasks { get; set; } = new List<TaskModel>();
    }
}
