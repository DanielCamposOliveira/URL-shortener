using API_Data.src.Data;
using API_Data.src.DTOs;
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



        


        // -- Adiciona uma nova URL ao banco de dados
        public async Task<OperationResult> RegisterUrlAsync(Url url)
        {
            try
            {
                // Adicionar a entidade ao banco de dados
                _db.Urls.Add(url);
                // Salvar as alterações no banco de dados
                await _db.SaveChangesAsync();
                return new OperationResult
                {
                    Success = true,
                    Message = "URL registrada com sucesso."
                };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }


        // -- Busca uma página de URLs associadas a um usuário pelo userId, com paginação
        public async Task<ExportPagUrlResponse> GetUrlPageAsync(string userId, int page, int limit)
        {
            try 
            {
                // Busca todas as URLs associadas ao usuário pelo userId, ordenadas por CreatedAt em ordem decrescente, com paginação
                var query = _db.Urls.Where(u => u.UserId == userId);

                // Conta o total de URLs associadas ao usuário
                var totalCount = await query.CountAsync();

                // Aplica paginação e seleciona os campos necessários para o modelo de exportação
                var data = await query
                     .OrderByDescending(u => u.CreatedAt)
                     .Skip((page - 1) * limit)
                     .Take(limit)
                     .Select(u => new PageUrlDTO
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
            catch (Exception ex) 
            {                
                return new ExportPagUrlResponse
                {
                    Urls = new List<PageUrlDTO>(),
                    Page = page,
                    Limit = limit,
                    TotalCount = 0
                };
            }

        }


        public async Task<Url?> GetUrlByIdAsync(string idOfuscado)
        {
            try
            {
                // Verifica se existe um usuário com o ID fornecido no banco de dados
                var url = await _db.Urls.FirstOrDefaultAsync(u => u.IdOfuscado == idOfuscado); 
                return url;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        // -- Remove uma URL do banco de dados pelo IdOfuscado
        public async Task<OperationResult> DeleteUrlAsync(string idOfuscado)
        {
            try
            {
                // Busca a URL pelo IdOfuscado no banco de dados
                var url = await _db.Urls.FirstOrDefaultAsync(u => u.IdOfuscado == idOfuscado);
                              
                if (url == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = "URL não encontrada."
                    };
                }

                // Remove a URL do banco de dados e salva as alterações
                _db.Urls.Remove(url);
                // Salva as alterações no banco de dados
                await _db.SaveChangesAsync();

                return new OperationResult
                {
                    Success = true,
                    Message = "URL removida com sucesso."
                };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }


        // -- Incrementa o contador de cliques e retorna a URL original pelo IdOfuscado
        public async Task<OperationResult> ClickUrlAsync(string idOfuscado)
        { 
            try
            {
                // Busca a URL pelo IdOfuscado no banco de dados
                var url = await _db.Urls.FirstOrDefaultAsync(u => u.IdOfuscado == idOfuscado && u.IsActive);
                              
                if (url == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = "URL não encontrada ou inativa."
                    };
                }

                // Incrementa o contador de cliques
                url.ClickCount++;
                // Atualiza a última data de acesso
                url.LastAccessedAt = DateTime.UtcNow;

                // Salva as alterações no banco de dados
                await _db.SaveChangesAsync();

                return new OperationResult
                {
                    Success = true,
                    Message = url.OriginalUrl
                };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }


        // -- Alterna o status de ativação de uma URL pelo IdOfuscado
        public async Task<OperationResult> DeactivateUrlAsync(string idOfuscado)
        {
            try
            {
                // Busca a URL pelo IdOfuscado no banco de dados
                var url = await _db.Urls.FirstOrDefaultAsync(u => u.IdOfuscado == idOfuscado);

                if (url == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = "URL não encontrada."
                    };
                }

                // Alterna o valor de IsActive
                url.IsActive = !url.IsActive;

                await _db.SaveChangesAsync();

                return new OperationResult
                {
                    Success = true,
                    Message = url.IsActive
                        ? "URL ativada com sucesso."
                        : "URL desativada com sucesso."
                };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

    }
}
