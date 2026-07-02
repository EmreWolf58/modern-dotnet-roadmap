using static System.Net.WebRequestMethods;

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

builder.Services.AddControllers();
/*
 
 */


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
//uygulamayı oluşturur.


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); //HTTP gelirse HTTPS'e yönlendir.

app.UseAuthorization();

app.MapControllers(); //Controller'ları kullan. Yani /api/tasks yada /api/users gibi endpointleri aktif et.

app.Run(); //Artık uygulamayı çalıştır. Bu satırdan sonra API istek kabul etmeye başlar.


