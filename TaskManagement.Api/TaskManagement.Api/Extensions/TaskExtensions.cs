
using TaskManagement.Api.Model.TaskModel;

namespace TaskManagement.Api.Extensions
{
    public static class TaskExtensions
    {
        public static string GetStatusText(this TaskModel task)
        {
            return task switch
            {
                { IsCompleted: true } => "Tamamlandı",
                _ => "Devam Ediyor"
            };
        }
    }
}
