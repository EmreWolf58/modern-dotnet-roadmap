using TaskManagement.Api.Model.TaskModels;

namespace TaskManagement.Api.Model
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int TaskId { get; set; }
        public TaskModel Task { get; set; } = null!;
    }
}
