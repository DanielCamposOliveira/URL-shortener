namespace API_Data.src.DTOs
{
    public class UserDtos
    {
        public record RegisterRequest(string Name, string Email, string Password); // Requisição de registro
        public record LoginRequest(string Email, string Password); // Requisição de login
        public record AuthResponse(string Token); // Retorna o token JWT

        public record QtdUrlMaxUserRequest(string UserId, string QtdMaxUrl);

        public record UserInfo
        {
            public string Name { get; init; }
            public bool IsAdmin { get; init; }
            public bool IsActive { get; init; }
            public bool isDarkMode { get; init; }
        }


        public class ListUsers
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public bool IsActive { get; set; }
            public bool IsAdmin { get; set; }
            public int QtdMaxUrl { get; set; }
        }

        public class ExportPagUserResponse
        {
            public List<ListUsers> User { get; set; } = new List<ListUsers>();
            public int Page { get; set; }
            public int Limit { get; set; }
            public int TotalCount { get; set; }
        }
    }
}
