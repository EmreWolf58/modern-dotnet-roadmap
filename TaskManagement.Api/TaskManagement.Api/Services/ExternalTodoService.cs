using System.Net.Http.Json;
using TaskManagement.Api.DTOS;
using TaskManagement.Api.Interfaces;

namespace TaskManagement.Api.Services
{
    public class ExternalTodoService : IExternalTodoService
    {
        private readonly HttpClient _httpClient;

        public ExternalTodoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ExternalTodoDto?> GetTodoAsync(int id, CancellationToken cancellationToken)
        {
            //var client = _httpClientFactory.CreateClient();
            //var response = await client.GetAsync($"https://jsonplaceholder.typicode.com/todos/{id}", cancellationToken);
            //response.EnsureSuccessStatusCode(); // başarısız status code varsa exception oluşturur.
            //return await response.Content.ReadFromJsonAsync<ExternalTodoDto>(cancellationToken: cancellationToken); //JSON'u C# nesnesine dönüştürür.

            //var client = _httpClient.GetAsync("JsonPlaceholder");
            var response = await _httpClient.GetAsync($"todos/{id}", cancellationToken);
            response.EnsureSuccessStatusCode(); // başarısız status code varsa exception oluşturur.
            return await response.Content.ReadFromJsonAsync<ExternalTodoDto>(cancellationToken: cancellationToken); //JSON'u C# nesnesine dönüştürür.

        }
    } 
}
