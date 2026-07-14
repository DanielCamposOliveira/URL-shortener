using System.Security.Cryptography;
using System.Text;

namespace API_Data.src.Utils
{
    public static class PasswordHasher
    {
        // Hashes a password using SHA256 and returns the hash as a hexadecimal string
        public static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }

        // Verifies  senha e contra um hash fornecido, calculando o hash da senha e comparando-o com o hash.
        public static bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    };
}
