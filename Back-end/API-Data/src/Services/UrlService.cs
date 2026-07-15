using API_Data.src.DTOs;
using API_Data.src.Models;
using API_Data.src.Repository;
using API_Data.src.Utils;
using static API_Data.src.DTOs.UserDtos;

namespace API_Data.src.Services
{
    /// <summary>
    /// Classe de Regra de Negócio
    /// </summary>
    /// <remarks>
    /// Esta classe é responsável por implementar a lógica de negócio relacionada à entidade Url.
    /// </remarks>
    /// 


    public class UrlService : IUrlService
    {
        private readonly IUrlRepository _urlRepository;
        private readonly IdGeneratorClient _idClient;
        private readonly IJwtService _jwtService;
        private readonly ILogger<UrlService> _logger;

        public UrlService(IUrlRepository urlRepository, IdGeneratorClient idClient, IJwtService jwtService, ILogger<UrlService> logger)
        {
            _urlRepository = urlRepository;
            _idClient = idClient;
            _jwtService = jwtService;
            _logger = logger;
        }

        // -- Registrar um novo usuário
        public async Task<IResult> PostRegisterUserAsync(RegisterRequest user)
        {
            // Registrar o usuário no repositório
            var result = await _urlRepository.RegisterUserAsync(user);

            // Verificar se o registro foi bem-sucedido
            if (!result.Success)
                return Results.BadRequest(new { message = result.Message });

            return Results.Created(); // 201 Created
        }



        // -- Busca o usuário pelo email e senha, e retorna um token JWT se a autenticação for bem-sucedida
        public async Task<IResult> PostAuthenticationUserAsync(LoginRequest req)
        {
            // Validar o email
            if (string.IsNullOrWhiteSpace(req.Email))
                return Results.BadRequest(new { message = "Email inválido." });

            // Consultar o repositório para obter o usuário correspondente ao email
            var user = await _urlRepository.GetUserByEmailAsync(req.Email);

            // Se o usuário não for encontrado, retornar null
            if (user == null)
                return Results.BadRequest(new { message = "Usuário não encontrado." });

            // Verificar a senha usando o PasswordHasher
            if (!PasswordHasher.VerifyPassword(req.Password, user.PasswordHash))
               return Results.BadRequest(new { message = "Senha incorreta." });

            // Gerar o token JWT usando o IJwtService
            var token = _jwtService.GenerateToken(user);

            // Retornar a resposta de autenticação com o token
            return Results.Ok(new AuthResponse(token));
        }


        // -- Criar uma nova URL encurtada para o usuário especificado
        public async Task<IResult> RegisterUrlAsync(string url, string userId)
        {
            // Validar a URL
            if (string.IsNullOrEmpty(url) || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
               return Results.BadRequest(new { message = "URL inválida." });

            // Gerar um ID único usando o IdGeneratorClient
            var idData = await _idClient.GenerateIdAsync();

            // Verificar se o ID foi gerado corretamente
            if (idData == null)
                return Results.Problem("Erro ao gerar ID único.", statusCode: 500);

            // Criar a entidade Url
            var entity = new Url
            {
                Id = idData.IdNumerico,
                IdOfuscado = idData.IdOfuscado,
                OriginalUrl = url,
                UserId = userId
            };

            // Salvar a entidade no repositório
            var result = await _urlRepository.RegisterUrlAsync(entity);

            // Verificar se a operação foi bem-sucedida
            if (!result.Success)
                return Results.BadRequest(new { message = result.Message });

            return Results.Created(); // 201 Created
        }


        // -- Obter uma lista paginada de URLs para um usuário específico
        public async Task<ExportPagUrlResponse> ObterPageUrlPorUserIdAsync(string userId, int page, int limit)
        {
            // Garantir que a página seja pelo menos 1
            if (page < 1) page = 1;
            // Garantir que o limite esteja entre 1 e 50, caso contrário, definir para 10
            if (limit < 1 || limit > 50) limit = 10;

            // Obter a lista de URLs paginadas para o usuário especificado
            var data = await _urlRepository.GetUrlPageAsync(userId, page, limit);           

            return data;
        }


   




    }
}
