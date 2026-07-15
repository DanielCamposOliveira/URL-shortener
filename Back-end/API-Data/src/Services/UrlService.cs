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

        public async Task<AuthResponse> ObterUsuarioPorEmailAsync(LoginRequest req)
        {
            // Validar o email
            if (string.IsNullOrEmpty(req.Email))
                throw new Exception("Email inválido.");

            // Consultar o repositório para obter o usuário correspondente ao email
            var user = await _urlRepository.GetUserByEmailAsync(req.Email);

            // Se o usuário não for encontrado, lançar uma exceção
            if (user == null)
                throw new Exception("Usuário não encontrado.");

            // Verificar a senha usando o PasswordHasher
            if (user == null || !PasswordHasher.VerifyPassword(req.Password, user.PasswordHash))
                throw new Exception("Senha inválida.");

            // Gerar o token JWT usando o IJwtService
            var token = _jwtService.GenerateToken(user);

            // Criar o objeto AuthResponse com o token gerado
            var User = new UserDtos.AuthResponse(token);


            return User;

        }

        public async Task<Url> CriarUrlAsync(string url, string userId)
        {
            // Validar a URL
            if (string.IsNullOrEmpty(url) || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
                throw new Exception("URL inválida.");

            // Gerar um ID único usando o IdGeneratorClient
            var idData = await _idClient.GenerateIdAsync();

            // Verificar se o ID foi gerado corretamente
            if (idData == null)
                throw new Exception("Erro ao gerar ID.");

            // Criar a entidade Url
            var entity = new Url
            {
                Id = idData.IdNumerico,
                IdOfuscado = idData.IdOfuscado,
                OriginalUrl = url,
                UserId = userId
            };

            // Salvar a entidade no repositório
            await _urlRepository.AddAsync(entity);

            return entity;
        }

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

        public async Task<IResult> PostRegisterUserAsync(RegisterRequest user)
        {
            try
            {
                // Registrar o usuário no repositório
                var result = await _urlRepository.RegisterUserAsync(user);

                // Verificar se o registro foi bem-sucedido
                if (!result.Success)
                    return Results.BadRequest(new { message = result.Message });

                return Results.Created(); // 201 Created
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        }





    }
}
