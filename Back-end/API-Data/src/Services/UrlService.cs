using API_Data.src.Models;
using API_Data.src.Repository;

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

        public UrlService( IUrlRepository urlRepository,  IdGeneratorClient idClient)
        {
            _urlRepository = urlRepository;
            _idClient = idClient;
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
    }
}
