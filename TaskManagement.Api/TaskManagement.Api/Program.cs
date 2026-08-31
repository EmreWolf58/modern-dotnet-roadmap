using AutoMapper;
using FluentValidation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using TaskManagement.Api.Events;
using TaskManagement.Api.Filters;
using TaskManagement.Api.HealthChecks;
using TaskManagement.Api.Interfaces;
using TaskManagement.Api.Mapping;
using TaskManagement.Api.Middlewares;
using TaskManagement.Api.Model.JwtSettingsModel;
using TaskManagement.Api.Responses;
using TaskManagement.Api.Services;
using TaskManagement.Api.Settings;
using TaskManagement.Api.Validators;
using static System.Net.WebRequestMethods;
using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using TaskManagement.Api.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);
/*
 Eskiden:
Global.asax
vardı.

Şimdi ise uygulama burada başlıyor.

Burada;
Configuration yükleniyor.
DI Container hazırlanıyor.
Logging hazırlanıyor.
Environment bilgisi yükleniyor.

Yani uygulamanın temel altyapısı hazırlanıyor.
 */

// Add services to the container.

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>(); //Controller action’ı çalışmadan önce devreye girer. ValidationFilter.cs
});
builder.Services.AddScoped<ActionLoggingFilter>();

builder.Services.AddScoped<TestExceptionFilter>();

builder.Services.AddScoped<ResultLoggingFilter>();

builder.Services.AddScoped<ResourceLoggingFilter>();

builder.Services.AddScoped<FirstActionFilter>();
builder.Services.AddScoped<SecondActionFilter>();

builder.Services.AddHealthChecks(); // Health Check'ler canlı ortamlarda (Azure, AWS, Kubernetes, Docker vb.) uygulamanın çalışır durumda olup olmadığını anlamak için kullanılır.

builder.Services.AddSingleton<ITaskService ,TaskService>(); // “Biri benden ITaskService isterse, ona TaskService ver.”
/*
 "TaskService sınıfını uygulamanın Dependency Injection (DI) sistemine kaydet."

builder: Uygulama başlatılırken kullanılan nesnedir.

var builder = WebApplication.CreateBuilder(args);  : Burada uygulamanın ayarlarını yapıyoruz.

Services: Uygulamadaki servislerin tutulduğu koleksiyondur. ("Benim kullanacağım servisler burada kayıtlı.")

AddSingleton: En önemli kısım burası.  TaskService'den uygulama boyunca sadece 1 tane oluştur.

eskiden new ile sen nesneyi oluşturursun.
Dependency Injection'da ise nesneyi ASP.NET Core oluşturur ve ihtiyacın olduğunda sana verir.
 */


builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationsSettings"));
/*
 bu kod ne yapar:
şunu buluyor.
ApplicationSettings
{
}
sonra bunu ApplicationSettings.cs içine dolduruyo yani jsonu class a döndürüyor.
 */

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt")); // bu kod ne yapar: şunu buluyor. JwtSettings { } sonra bunu JwtSettings.cs içine dolduruyo yani jsonu class a döndürüyor.
builder.Services.AddScoped<IJwtService, JwtService>(); // “Biri benden IJwtService isterse, ona JwtService ver. Burada Scoped kullanabiliriz. Servis her HTTP isteği için bir kez oluşturulur.

//Gelen token Issuer doğru mu? Audience doğru mu? İmza doğru mu? Süresi dolmuş mu? Hepsi doğruysa Kullanıcı sisteme giriş yapmış.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration
            .GetSection("Jwt")
            .Get<JwtSettings>();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings!.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key)),

            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddValidatorsFromAssemblyContaining<CreateTaskDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateTaskDtoValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    #region version için ekliyorum
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaskManagement.Api",
        Version = "v1"
    });

    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "TaskManagement.Api",
        Version = "v2"
    });

    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        return apiDesc.GroupName == docName;
    });
    #endregion
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Token giriniz."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAutoMapper(cfg => { },typeof(TaskMappingProfile));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OnlyAdmins", policy =>
    {
        policy.RequireRole("Admin");
    });
});

builder.Services.AddScoped<ValidationFilter>();

builder.Services.Configure<ApiBehaviorOptions>(
    options =>
    {
        options.InvalidModelStateResponseFactory =
            context =>
            {
                var errors = context.ModelState
                    .Where(item =>
                        item.Value?.Errors.Count > 0)
                    .ToDictionary(
                        item => item.Key,
                        item => item.Value!.Errors
                            .Select(error =>
                                string.IsNullOrWhiteSpace(
                                    error.ErrorMessage)
                                    ? "Geçersiz bir değer gönderildi."
                                    : error.ErrorMessage)
                            .ToArray());

                var response = new ApiErrorResponse
                {
                    Message =
                        "Gönderilen request geçersiz.",
                    StatusCode =
                        StatusCodes.Status400BadRequest,
                    Errors = errors,
                    Path =
                        context.HttpContext.Request.Path,
                    TraceId =
                        context.HttpContext.TraceIdentifier
                };

                return new BadRequestObjectResult(response);
            };
    });

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection("CacheSettings"));

builder.Services.AddHealthChecks()
    .AddCheck<ConfigurationHealthCheck>("Application")
    .AddCheck<ConfigurationHealthCheck>("Configuration");

builder.Services.AddSingleton<TaskEventPublisher>();
builder.Services.AddSingleton<TaskEventSubscriber>();


//version
builder.Services
    .AddApiVersioning(option =>
    {
        option.DefaultApiVersion = new ApiVersion(1, 0);
        option.AssumeDefaultVersionWhenUnspecified = true;
        option.ReportApiVersions = true;

        option.ApiVersionReader =
            new HeaderApiVersionReader("X-Api-Version");
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
    });

//policies
builder.Services.AddCors(options => 
{
    options.AddPolicy("TaskManagementPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader();
        //.WithMethods(   //bu policies örneğiyle kısıtlamaları arttırabiliyosun.
        //    HttpMethods.Get,
        //    HttpMethods.Post)
        //.WithHeaders(
        //    "Content-Type",
        //    "Authorization",
        //    "X-Api-Version"
        //    );

    });
});
/*
AddCors(): CORS servislerini DI container'a ekledik.
AddPolicy(): Kendi CORS policy'mizi oluşturduk.
WithOrigins(): Sadece bu origin'e izin veriyoruz. Yani örneğimizde gelecekte:
*/

//fixed windows ekledim.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromSeconds(10);
        limiterOptions.QueueLimit = 0;
    });

    options.AddSlidingWindowLimiter("sliding", limiterOptions =>
    {
        limiterOptions.PermitLimit = 6;
        limiterOptions.Window = TimeSpan.FromSeconds(12);
        limiterOptions.SegmentsPerWindow = 3;
        limiterOptions.QueueLimit = 0;
    });

    options.AddTokenBucketLimiter("token", limiterOptions =>
    {
        limiterOptions.TokenLimit = 5; //Kovanın maksimum token kapasitesi.
        limiterOptions.TokensPerPeriod = 2; //Her yenilenme döneminde kaç token ekleneceğini belirtir.
        limiterOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(5); //Tokenların ne sıklıkla yenileneceğini belirtir.
        limiterOptions.AutoReplenishment = true; //Tokenların otomatik olarak yenilenmesini sağlar.
        limiterOptions.QueueLimit = 0;
    });
    options.AddConcurrencyLimiter("concurrency", limiterOptions =>
    {
        limiterOptions.PermitLimit = 2; //aynı anda maksimum 2 request işlenebilir.
        limiterOptions.QueueLimit = 0; //fazla request bekletilmez.
    });

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>( //GlobalLimiter:Rate Limiting'i sadece attribute koyduğum endpoint'lere değil, global olarak uygula.
        HttpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey:
            HttpContext.Connection.RemoteIpAddress?.ToString()
            ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 20,
                Window = TimeSpan.FromSeconds(10),
                QueueLimit = 0
            }));
});

builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("TasksPolicy", builder =>
    {
        builder.Expire(TimeSpan.FromSeconds(30))
        .SetVaryByQuery("*")
        .Tag("tasks");
    });
});

//cache eklendi.
builder.Services.AddMemoryCache(); //MemoryCache'i DI container'a ekledik. Artık uygulama boyunca MemoryCache'i kullanabiliriz

//background service ekledim.
builder.Services.AddHostedService<ApplicationLifetimeHostedService>(); //Uygulama başlatıldığında ve durdurulduğunda loglama yapacak. BackgroundService.cs
/*
 AddHostedService<T>() şu anlama geliyor:
Bu sınıf uygulamanın Hosted Service'lerinden biridir. Uygulama başlarken başlat, kapanırken durdur.
 */


var app = builder.Build();
//uygulamayı oluşturur.

app.Services.GetRequiredService<TaskEventSubscriber>();

app.UseMiddleware<ExceptionMiddleware>(); //Bu yüzden hata yakalama middleware'i genellikle pipeline'ın başında bulunur.
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "TaskManagement.Api v1");

        options.SwaggerEndpoint(
            "/swagger/v2/swagger.json",
            "TaskManagement.Api v2");
    });
}

app.UseHttpsRedirection(); //HTTP gelirse HTTPS'e yönlendir.

app.UseResponseCaching();
app.UseOutputCache(); //OutputCache'i kullan. Yani cache'lenmiş response varsa onu döndür, yoksa action çalıştır.

app.UseCors("TaskManagementPolicy"); //Biraz önce oluşturduğum TaskManagementPolicy isimli CORS kurallarını kullan.

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter(); //Rate Limiting'i kullan. Yani 10 saniyede 5 istekten fazlasını kabul etme.

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter= UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapControllers(); //Controller'ları kullan. Yani /api/tasks yada /api/users gibi endpointleri aktif et.

app.Run(); //Artık uygulamayı çalıştır. Bu satırdan sonra API istek kabul etmeye başlar.


