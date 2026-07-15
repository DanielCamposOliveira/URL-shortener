using API_Data.src.DTOs;
using API_Data.src.Models;
using static API_Data.src.DTOs.UserDtos;

namespace API_Data.src.Services
{
    public interface IUrlService
    {
        Task<AuthResponse> ObterUsuarioPorEmailAsync(LoginRequest req);
        Task<Url> CriarUrlAsync(string url, string userId);

        Task<ExportPagUrlResponse> ObterPageUrlPorUserIdAsync(string userId, int page, int limit);


        Task<IResult> PostRegisterUserAsync(RegisterRequest User);

    }
}
