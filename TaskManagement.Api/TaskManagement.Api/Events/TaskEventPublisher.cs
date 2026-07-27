namespace TaskManagement.Api.Events
{
    public class TaskEventPublisher
    {
        public event Action<string>? TaskCreated;
        public void PublishTaskCreated(string title)
        {
            TaskCreated?.Invoke(title);
        }
    }
}
