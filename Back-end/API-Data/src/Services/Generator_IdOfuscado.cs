using API_Data.src.Services.Interface;

namespace API_Data.src.Services
{
    public record IdGeneratorResponse(long IdNumerico, string IdOfuscado, int MaquinaOrigem);
    public class Generator_IdOfuscado : IGenerator_IdOfuscado
    {
        private readonly HttpClient _httpClient;

        public Generator_IdOfuscado(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IdGeneratorResponse?> GenerateIdAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IdGeneratorResponse>("/api/v1/identificadores");
            }
            catch
            {
                return null;
            }
        }
    }
}
