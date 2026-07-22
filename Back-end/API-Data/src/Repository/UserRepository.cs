using API_Data.src.Data;
using API_Data.src.DTOs;
using API_Data.src.Models;
using API_Data.src.Repository.Interface;
using API_Data.src.Utils;
using Microsoft.EntityFrameworkCore;
using static API_Data.src.DTOs.UserDtos;

namespace API_Data.src.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        // -- Busca um usuário pelo email no banco de dados
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                // Verifica se existe um usuário com o email fornecido no banco de dados
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // -- Busca um usuário pelo ID no banco de dados
        public async Task<User?> GetUserByIdAsync(string userId)
        {
            try
            {
                // Verifica se existe um usuário com o ID fornecido no banco de dados
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        // -- Registra um novo usuário no banco de dados
        public async Task<OperationResult> RegisterUserAsync(RegisterRequest user)
        {
            try
            {
                // Verifica se o e-mail já está cadastrado no banco de dados
                if (await _db.Users.AnyAsync(u => u.Email == user.Email))
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = "E-mail já cadastrado."
                    };
                }

                // Cria um novo usuário com os dados fornecidos e o hash da senha
                var newUser = new User
                {
                    Name = user.Name,
                    Email = user.Email,
                    PasswordHash = PasswordHasher.HashPassword(user.Password)
                };

                // Adiciona o usuário ao banco de dados
                _db.Users.Add(newUser);
                //salva as alterações
                await _db.SaveChangesAsync();

                return new OperationResult
                {
                    Success = true,
                    Message = "Usuário cadastrado com sucesso."
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

        public async Task<OperationResult> DeleteUserAsync(string userId)
        {
            try
            {
                // Busca a URL pelo IdOfuscado no banco de dados
                var User = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (User == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = "Usuario não encontrada."
                    };
                }

                // Remove a URL do banco de dados e salva as alterações
                _db.Users.Remove(User);
                // Salva as alterações no banco de dados
                await _db.SaveChangesAsync();

                return new OperationResult
                {
                    Success = true,
                    Message = "Usuario removida com sucesso."
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

        public async Task<OperationResult> DeactivateUserAsync(string userId)
        {
            try
            {
                // Busca a URL pelo IdOfuscado no banco de dados
                var _user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (_user == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = "Usuario não encontrada."
                    };
                }

                // Alterna o valor de IsActive
                _user.IsActive = !_user.IsActive;

                await _db.SaveChangesAsync();

                return new OperationResult
                {
                    Success = true,
                    Message = _user.IsActive
                        ? "Usuario ativada com sucesso."
                        : "Usuario desativada com sucesso."
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

        public async Task<OperationResult> ThemeUser(string userId, bool isDarkMode)
        {
            try
            {
                // Busca a URL pelo IdOfuscado no banco de dados
                var _user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (_user == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = "Usuario não encontrada."
                    };
                }

                // Alterna o valor de IsActive
                _user.isDarkMode = isDarkMode;

                await _db.SaveChangesAsync();

                return new OperationResult
                {
                    Success = true,
                    Message ="DarkMode:" + _user.isDarkMode
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

        public async Task<ExportPagUserResponse> GetUserPageAsync(string userId, int page, int limit)
        {           

            try
            {
                // Busca todos os usuários
                var query = _db.Users.AsQueryable();

                // Conta o total de usuários
                var totalCount = await query.CountAsync();

                // Aplica paginação e seleciona os campos necessários para o modelo de exportação
                var data = await query
                    .OrderByDescending(u => u.Id)
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .Select(u => new ListUsers
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Email = u.Email,
                        IsActive = u.IsActive,
                        IsAdmin = u.IsAdmin,
                        QtdMaxUrl = u.QtdMaxUrl,
                    })
                    .ToListAsync();

                // Retorna a resposta com os dados da página, limite e contagem total
                return new ExportPagUserResponse
                {
                    User = data,
                    Page = page,
                    Limit = limit,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                return new ExportPagUserResponse
                {
                    User = new List<ListUsers>(),
                    Page = page,
                    Limit = limit,
                    TotalCount = 0
                };
            }

        }

        public async Task<OperationResult> QtdUrlMaxUser(string userId, int QtdMaxUrl)
        {
            try
            {
                // Busca a URL pelo IdOfuscado no banco de dados
                var _user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (_user == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = "Usuario não encontrada."
                    };
                }

                // Alterna o valor de IsActive
                _user.QtdMaxUrl = QtdMaxUrl;

                await _db.SaveChangesAsync();

                return new OperationResult
                {
                    Success = true,
                    Message = "QtdMaxUrl:" + _user.QtdMaxUrl
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
