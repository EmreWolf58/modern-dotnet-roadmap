# Task Management API

ASP.NET Core 8 kullanılarak geliştirilen, görev yönetimi işlemlerini gerçekleştiren örnek bir Web API projesidir.

Bu proje ASP.NET Core temellerini, katmanlı kod organizasyonunu, kimlik doğrulamayı, doğrulamayı, middleware kullanımını ve API kalite standartlarını uygulamalı olarak öğrenmek amacıyla geliştirilmiştir.

> Proje Faz 1 kapsamında In-Memory veri ile çalışmaktadır. SQL Server ve Entity Framework Core kullanılmamaktadır.

## Kullanılan Teknolojiler

- .NET 8
- ASP.NET Core Web API
- Swagger / OpenAPI
- Dependency Injection
- AutoMapper
- FluentValidation
- JWT Authentication
- Role ve Policy tabanlı Authorization
- Serilog
- Health Checks
- In-Memory veri yapısı

## Proje Özellikleri

- Task oluşturma
- Task listeleme
- ID ile task getirme
- Task güncelleme
- Soft delete
- Pagination
- Filtering
- Sorting
- JWT token üretimi
- Yetkilendirilmiş endpoint'ler
- Role ve policy kontrolleri
- Global exception handling
- Standart API response yapısı
- Request logging
- FluentValidation kuralları
- Health Check endpoint'i
- Development ve Production ortam ayarları

## Proje Yapısı

```text
TaskManagement.Api
├── Controllers
├── DTOs
├── Events
├── Extensions
├── Filters
├── HealthChecks
├── Helpers
├── Interfaces
├── Mapping
├── Middlewares
├── Models
├── Responses
├── Services
├── Settings
├── Validators
├── Program.cs
└── appsettings.json
```

## API Endpoint'leri

### Authentication

```http
POST /api/auth/login
```

### Tasks

```http
GET    /api/tasks
GET    /api/tasks/{id}
POST   /api/tasks
PUT    /api/tasks/{id}
DELETE /api/tasks/{id}
```

### Health Check

```http
GET /health
```

Endpoint adresleri controller route tanımlarına göre farklılık gösterebilir.

## Listeleme Parametreleri

Task listeleme endpoint'i pagination, filtering ve sorting destekler.

```http
GET /api/tasks?page=1&pageSize=10
```

```http
GET /api/tasks?search=api
```

```http
GET /api/tasks?completed=false
```

```http
GET /api/tasks?sortBy=createdDate&descending=true
```

```http
GET /api/tasks?includeDeleted=true
```

Parametreler birlikte de kullanılabilir:

```http
GET /api/tasks?page=1&pageSize=5&completed=false&sortBy=createdDate&descending=true
```

## Soft Delete

DELETE işlemi task kaydını fiziksel olarak kaldırmaz.

Silinen kayıt:

```text
IsDeleted = true
DeletedDate = silinme tarihi
```

olarak işaretlenir.

Silinen kayıtları da listelemek için:

```http
GET /api/tasks?includeDeleted=true
```

kullanılabilir.

## Projeyi Çalıştırma

Repository'yi klonlayın:

```bash
git clone https://github.com/EmreWolf58/modern-dotnet-roadmap
```

Proje klasörüne gidin:

```bash
cd modern-dotnet-roadmap/TaskManagement.Api
```

Bağımlılıkları yükleyin:

```bash
dotnet restore
```

Projeyi çalıştırın:

```bash
dotnet run
```

Development ortamında Swagger arayüzünü açın:

```text
https://localhost:7214/swagger
```


## JWT Ayarı

JWT anahtarı geliştirme ortamında User Secrets ile tanımlanabilir:

```bash
dotnet user-secrets init
dotnet user-secrets set "JwtSettings:Key" "en-az-32-karakterlik-gelistirme-anahtari"
```

Ayar adları projedeki `JwtSettings` sınıfına göre düzenlenmelidir.

## Örnek Response

```json
{
  "success": true,
  "message": "Task listesi başarıyla getirildi.",
  "data": {
    "page": 1,
    "pageSize": 10,
    "totalCount": 5,
    "totalPages": 1,
    "items": []
  }
}
```

## Projenin Mevcut Durumu

Faz 1 tamamlanma aşamasındadır.

Bu fazda:

- ASP.NET Core temelleri
- Dependency Injection
- Middleware
- Authentication ve Authorization
- Validation
- AutoMapper
- Logging
- Health Checks
- Modern C#
- Pagination
- Filtering
- Sorting
- Soft Delete

konuları uygulanmıştır.

Bir sonraki fazda Entity Framework Core ve SQL Server entegrasyonu yapılacaktır.