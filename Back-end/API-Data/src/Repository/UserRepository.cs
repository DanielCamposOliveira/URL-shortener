using API_Data.src.Data;
using API_Data.src.Models;
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
        public async Task<User?> GetUserByIdAsync(string id)
        {
            try
            {
                // Verifica se existe um usuário com o ID fornecido no banco de dados
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
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
    }
}
