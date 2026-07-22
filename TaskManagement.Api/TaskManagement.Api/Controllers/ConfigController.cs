using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TaskManagement.Api.Settings;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigController : ControllerBase
    {
        private readonly MailSettings _mailSettings;
        private readonly CacheSettings _cacheSettings;
        private  readonly IConfiguration _configuration;
        private readonly ApplicationSettings _applicationSettings;
        private readonly IWebHostEnvironment _environment;

        public ConfigController (IOptions<MailSettings> mailOptions, IOptions<CacheSettings> cacheOptions, IConfiguration configuration, IOptions<ApplicationSettings> applicationOptions, IWebHostEnvironment environment  )
        {
            _mailSettings = mailOptions.Value;
            _cacheSettings = cacheOptions.Value;
            _configuration = configuration;
            _applicationSettings = applicationOptions.Value;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var response = new
            {
                Environment = new
                {
                    EnvironmentName = _environment.EnvironmentName,
                    IsDevelopment = _environment.IsDevelopment(),
                    IsProduction = _environment.IsProduction()

                },
                OptionsPattern = new
                {
                    ApplicationName = _applicationSettings.ApplicationName,
                    MaxTaskCount = _applicationSettings.MaxTaskCount,
                    MailHost = _mailSettings.Host,
                    MailPort = _mailSettings.Port,
                    MailUsername = _mailSettings.UserName,
                    CacheEnabled = _cacheSettings.Enabled,
                    CacheExpireMinutes = _cacheSettings.ExpireMinutes,
                },
                Configuration = new
                {
                    MailHost = _configuration["MailSettings:Host"],
                    MailPort = _configuration.GetValue<int>("MailSettings:Port"),
                    MailUsername = _configuration["MailSettings:UserName"],
                    CacheEnabled = _configuration.GetValue<bool>("CacheSettings:Enabled"),
                    CacheExpireMinutes = _configuration.GetValue<int>("CacheSettings:ExpireMinutes"),
                }
            };

            return Ok(response);
        }
    }
}
