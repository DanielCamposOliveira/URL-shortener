namespace API_Data.src.DTOs
{
    public class UserDtos
    {
        public record RegisterRequest(string Name, string Email, string Password); // Requisição de registro
        public record LoginRequest(string Email, string Password); // Requisição de login
        public record AuthResponse(string Token); // Retorna o token JWT

        public record UserInfo
        {
            public string Name { get; init; }
            public bool IsAdmin { get; init; }
            public bool IsActive { get; init; }
        }
    }
}
