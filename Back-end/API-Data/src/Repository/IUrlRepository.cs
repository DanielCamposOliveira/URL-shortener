using API_Data.src.Models;

namespace API_Data.src.Repository
{
    public interface IUrlRepository
    {
        Task<User?> GetUserByEmailAsync(string email);

        Task AddAsync(Url url);
        Task<Url?> GetByIdOfuscadoAsync(string id);
    }
}
