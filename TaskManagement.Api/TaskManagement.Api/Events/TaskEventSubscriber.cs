namespace TaskManagement.Api.Events
{
    public class TaskEventSubscriber
    {
        private readonly ILogger<TaskEventSubscriber> _logger;

        public TaskEventSubscriber(TaskEventPublisher publisher, ILogger<TaskEventSubscriber> logger)
        {
            _logger = logger;
            publisher.TaskCreated += OnTaskCreated;
        }

        private void OnTaskCreated(string title)
        {
            _logger.LogInformation("Event Yakalandı. Yeni task oluşturuldu: {Title}",title);
        }
    }
}
