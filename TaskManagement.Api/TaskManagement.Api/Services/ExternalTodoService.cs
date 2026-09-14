using System.Net.Http.Json;
using TaskManagement.Api.DTOS;
using TaskManagement.Api.Interfaces;

namespace TaskManagement.Api.Services
{
    public class ExternalTodoService : IExternalTodoService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExternalTodoService> _logger;

        public ExternalTodoService(HttpClient httpClient, ILogger<ExternalTodoService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ExternalTodoDto?> GetTodoAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                //var client = _httpClientFactory.CreateClient();
                //var response = await client.GetAsync($"https://jsonplaceholder.typicode.com/todos/{id}", cancellationToken);
                //response.EnsureSuccessStatusCode(); // başarısız status code varsa exception oluşturur.
                //return await response.Content.ReadFromJsonAsync<ExternalTodoDto>(cancellationToken: cancellationToken); //JSON'u C# nesnesine dönüştürür.

                //var client = _httpClient.GetAsync("JsonPlaceholder");
                var response = await _httpClient.GetAsync($"todos/{id}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"External API request failed. Status code: {response.StatusCode}", null, response.StatusCode);
                }

                //response.EnsureSuccessStatusCode(); // başarısız status code varsa exception oluşturur.
                return await response.Content.ReadFromJsonAsync<ExternalTodoDto>(cancellationToken: cancellationToken); //JSON'u C# nesnesine dönüştürür.
            }
            catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested) //Bu sayede gerçek cancellation'ı timeout sanmıyoruz. when içindeki kod ile.
            {
                _logger.LogWarning(ex , "External API request timed out for Todo Id: {TodoId}" , id);
                throw new TimeoutException("External API request timed out.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "External API request failed for Todo Id: {TodoId}", id);
                throw;
            }

        }
    } 
}
