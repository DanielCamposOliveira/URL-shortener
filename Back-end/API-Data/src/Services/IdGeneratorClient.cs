using System.Net.Http.Json;

namespace API_Data.src.Services
{
    public record IdGeneratorResponse(long IdNumerico, string IdOfuscado, int MaquinaOrigem);

    public class IdGeneratorClient
    {
        private readonly HttpClient _httpClient;

        public IdGeneratorClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IdGeneratorResponse?> GenerateIdAsync()
        {
            try
            {
                // Consome a rota descrita na API 1
                return await _httpClient.GetFromJsonAsync<IdGeneratorResponse>("/api/v1/identificadores");
            }
            catch
            {
                return null;
            }
        }
    }
}
