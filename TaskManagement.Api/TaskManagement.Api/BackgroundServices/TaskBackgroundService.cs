namespace TaskManagement.Api.BackgroundServices
{
    public class TaskBackgroundService: BackgroundService
    {
        private readonly ILogger<TaskBackgroundService> _logger;

        public TaskBackgroundService(ILogger<TaskBackgroundService> logger)
        {
            _logger = logger;
        }
        //protected: Bu metodun dışarıdan normal şekilde çağrılması için tasarlanmadığını gösteriyor. yani backgroundService.ExecuteAsync(); yapmak için değil.

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) //Bu bizim worker'ımızın asıl çalışma metodu. Uygulama çalışırken arka planda ne yapacaksın? sorusunun cevabını buraya yazıyoruz. 
        {
            _logger.LogInformation("TaskBackgroundService çalışmaya başladı.");

            try
            {
                while (!stoppingToken.IsCancellationRequested) //Uygulamadan "dur" sinyali gelmediği sürece çalışmaya devam et.
                {
                    _logger.LogInformation("Background task çalışıyor. Saat: {Time}", DateTime.Now);

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); //döngü inanılmaz hızlı döner bunu engellemek için delay attık.
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
