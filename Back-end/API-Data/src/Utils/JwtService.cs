using API_Data.src.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_Data.src.Utils
{
    /// <summary>
    /// Classe de Serviço JWT
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly byte[] _keyBytes;

        public JwtService(IConfiguration configuration)
        {
            var key = configuration["Jwt:Key"]
                ?? throw new Exception("Chave JWT não configurada.");

            _keyBytes = Encoding.UTF8.GetBytes(key);
        }


        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                }),

                Expires = DateTime.UtcNow.AddHours(2),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(_keyBytes),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}
