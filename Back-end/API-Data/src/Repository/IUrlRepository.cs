using API_Data.src.DTOs;
using API_Data.src.Models;
using static API_Data.src.DTOs.UserDtos;

namespace API_Data.src.Repository
{
    public interface IUrlRepository
    {
        Task<User?> GetUserByEmailAsync(string email);

        Task AddAsync(Url url);
      
        Task<ExportPagUrlResponse> GetUrlPageAsync(string userId, int page, int limit);

        Task<OperationResult> RegisterUserAsync(RegisterRequest user);

    }
}
