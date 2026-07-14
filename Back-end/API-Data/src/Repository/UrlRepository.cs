using API_Data.src.Data;
using API_Data.src.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Data.src.Repository
{
    /// <summary>
    /// Classe de Repositório Banco de Dados
    /// </summary>
    /// <remarks>
    /// Esta classe é responsável por interagir com o banco de dados para operações relacionadas à entidade Url.
    /// </remarks>
    /// 

    public class UrlRepository : IUrlRepository
    {
        private readonly AppDbContext _db;

        public UrlRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Url url)
        {
            // Adicionar a entidade ao banco de dados
            _db.Urls.Add(url);
            // Salvar as alterações no banco de dados
            await _db.SaveChangesAsync();
        }




        // Busca um usuário pelo email no banco de dados
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

    }
}
