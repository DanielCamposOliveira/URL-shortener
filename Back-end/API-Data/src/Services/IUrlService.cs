using API_Data.src.Models;

namespace API_Data.src.Services
{
    public interface IUrlService
    {
        Task<Url> CriarUrlAsync(string url, string userId);
    }
}
