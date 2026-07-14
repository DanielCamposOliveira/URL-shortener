using API_Data.src.Data;
using API_Data.src.DTOs;
using API_Data.src.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        // Busca uma página de URLs associadas a um usuário pelo userId, com paginação
        public async Task<ExportPagUrlResponse> GetUrlPageAsync(string userId, int page, int limit)
        {
            // Busca todas as URLs associadas ao usuário pelo userId, ordenadas por CreatedAt em ordem decrescente, com paginação
            var query =  _db.Urls.Where(u => u.UserId == userId);
            
            // Conta o total de URLs associadas ao usuário
            var totalCount = await query.CountAsync();

            // Aplica paginação e seleciona os campos necessários para o modelo de exportação
            var data = await query
                 .OrderByDescending(u => u.CreatedAt)
                 .Skip((page - 1) * limit)
                 .Take(limit)
                 .Select(u => new ExportPagUrlDTO
                 {
                     IsActive = u.IsActive,
                     ClickCount = u.ClickCount,
                     ExpiresAt = u.ExpiresAt,
                     LastAccessedAt = u.LastAccessedAt,
                     IdOfuscado = u.IdOfuscado,
                     OriginalUrl = u.OriginalUrl
                 })
                 .ToListAsync();

            // Retorna a resposta com os dados da página, limite e contagem total
            return new ExportPagUrlResponse
            {
                Urls = data,
                Page = page,
                Limit = limit,
                TotalCount = totalCount
            };
        }






    }
}
