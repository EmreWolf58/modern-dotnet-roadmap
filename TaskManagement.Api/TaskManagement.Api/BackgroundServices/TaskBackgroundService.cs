using TaskManagement.Api.DTOS;
using TaskManagement.Api.Interfaces;

namespace TaskManagement.Api.BackgroundServices
{
    public class TaskBackgroundService: BackgroundService
    {
        private readonly ILogger<TaskBackgroundService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public TaskBackgroundService(ILogger<TaskBackgroundService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }
        //protected: Bu metodun dışarıdan normal şekilde çağrılması için tasarlanmadığını gösteriyor. yani backgroundService.ExecuteAsync(); yapmak için değil.

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) //Bu bizim worker'ımızın asıl çalışma metodu. Uygulama çalışırken arka planda ne yapacaksın? sorusunun cevabını buraya yazıyoruz. 
        {
            _logger.LogInformation("TaskBackgroundService çalışmaya başladı.");

            try
            {
                using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    using var scope = _scopeFactory.CreateScope(); //scope oluşturduk. scope sayesinde bağımlılıkları yönetebiliyoruz. scope'u using ile sarmaladık ki işimiz bittiğinde dispose edilsin
                    var taskService = scope.ServiceProvider.GetRequiredService<ITaskService>();
                    var query = new TaskQuery();

                    var result = taskService.GetAll(query);


                    _logger.LogInformation("Background Service task kontrolü yaptı. Task sayısı: {TaskCount}", result.TotalCount);

                    //await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); 
                    //döngü inanılmaz hızlı döner bunu engellemek için delay attık.
                    //await olmazsa thread bloke olur ve uygulama yanıt vermez. await ile thread bloke olmaz, thread başka iş yapabilir.
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {//OperationCanceledException oluştuysa ve bizim stoppingToken'ımız gerçekten cancellation aldıysa bunu yakala.
                //Bu sayede gerçek bir hatayla normal kapanmayı birbirine karıştırmıyoruz.
                _logger.LogInformation("TaskBackgroundService iptal sinyali aldı.");
            }
            _logger.LogInformation("TaskBackgroundService ExecuteAsync tamamlandı.");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("TaskBackgroundService kapanıyor.");

            await base.StopAsync(cancellationToken);

            _logger.LogInformation("TaskBackgroundService kapandı.");
        }
    }
}
