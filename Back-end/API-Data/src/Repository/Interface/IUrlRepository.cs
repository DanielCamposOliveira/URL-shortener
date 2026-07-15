using API_Data.src.DTOs;
using API_Data.src.Models;

namespace API_Data.src.Repository.Interface
{
    public interface IUrlRepository
    {
        Task<OperationResult> RegisterUrlAsync(Url url);
      
        Task<ExportPagUrlResponse> GetUrlPageAsync(string userId, int page, int limit);

        Task<OperationResult> DeleteUrlAsync(string idOfuscado);

        Task<Url?> GetUrlByIdAsync(string idOfuscado);

        Task<OperationResult> ClickUrlAsync(string idOfuscado);

        Task<OperationResult> DeactivateUrlAsync(string idOfuscado);
    }
}
