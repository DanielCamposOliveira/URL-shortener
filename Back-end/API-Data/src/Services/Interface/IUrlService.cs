using API_Data.src.DTOs;
using API_Data.src.Models;

namespace API_Data.src.Services.Interface
{
    public interface IUrlService
    {
      
        Task<IResult> RegisterUrlAsync(string url, string userId);

        Task<ExportPagUrlResponse> ObterPageUrlPorUserIdAsync(string userId, int page, int limit);
        
        Task<IResult> DeleteUrlAsync(string userId, string idOfuscado);

        Task<IResult> DeactivateUrlAsync(string userId, string idOfuscado);

        Task<OperationResult> GetUrlByIdAsync(string idOfuscado);
               

    }
}
