using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TaskManagement.Api.Interfaces;
using TaskManagement.Api.Middlewares;
using TaskManagement.Api.Model.JwtSettingsModel;
using TaskManagement.Api.Services;
using TaskManagement.Api.Settings;
using static System.Net.WebRequestMethods;
using FluentValidation;
using TaskManagement.Api.Validators;
using TaskManagement.Api.Mapping;
using AutoMapper;
using TaskManagement.Api.Filters;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.Responses;
using Serilog;
using TaskManagement.Api.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

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

var app = builder.Build();
//uygulamayı oluşturur.

app.UseMiddleware<ExceptionMiddleware>(); //Bu yüzden hata yakalama middleware'i genellikle pipeline'ın başında bulunur.
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); //HTTP gelirse HTTPS'e yönlendir.

app.UseAuthentication();

app.UseAuthorization();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter= UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapControllers(); //Controller'ları kullan. Yani /api/tasks yada /api/users gibi endpointleri aktif et.

app.Run(); //Artık uygulamayı çalıştır. Bu satırdan sonra API istek kabul etmeye başlar.


