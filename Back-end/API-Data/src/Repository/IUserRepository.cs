using API_Data.src.Models;
using static API_Data.src.DTOs.UserDtos;

namespace API_Data.src.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(string id);

        Task<OperationResult> RegisterUserAsync(RegisterRequest user);

        Task<User?> GetUserByEmailAsync(string email);
    }
}
