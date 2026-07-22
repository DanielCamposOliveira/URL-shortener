using static API_Data.src.DTOs.UserDtos;

namespace API_Data.src.Services.Interface
{
    public interface IUserService
    {
        Task<IResult> PostRegisterUserAsync(RegisterRequest User);

        Task<IResult> PostAuthenticationUserAsync(LoginRequest req);

        Task<IResult> DeactivateUserAsync(string userId, string UserActiver);

        Task<IResult> DeleteUser(string userId, string UserDelete);

        Task<UserInfo> GetUserInfo(string userId);
    }
}
