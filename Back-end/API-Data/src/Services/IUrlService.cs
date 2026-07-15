using API_Data.src.DTOs;
using API_Data.src.Models;
using static API_Data.src.DTOs.UserDtos;

namespace API_Data.src.Services
{
    public interface IUrlService
    {
        Task<IResult> PostAuthenticationUserAsync(LoginRequest req);

        Task<IResult> RegisterUrlAsync(string url, string userId);

        Task<ExportPagUrlResponse> ObterPageUrlPorUserIdAsync(string userId, int page, int limit);

        Task<IResult> PostRegisterUserAsync(RegisterRequest User);

        Task<IResult> DeleteUrlAsync(string userId, string idOfuscado);

        Task<OperationResult> GetUrlByIdAsync(string idOfuscado);

        Task<IResult> DeactivateUrlAsync(string userId, string idOfuscado);

    }
}
