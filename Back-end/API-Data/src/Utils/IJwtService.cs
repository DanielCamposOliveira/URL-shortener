using API_Data.src.Models;

namespace API_Data.src.Utils
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
