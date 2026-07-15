using API_Data.src.Data;
using API_Data.src.DTOs;
using API_Data.src.Extensions;
using API_Data.src.Models;
using API_Data.src.Repository;
using API_Data.src.Services;
using API_Data.src.Utils;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using static API_Data.src.DTOs.UserDtos;
Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<IUrlRepository, UrlRepository>();
builder.Services.AddScoped<IUrlService, UrlService>();
builder.Services.AddScoped<IJwtService, JwtService>();



// Recupera a string de conexão do appsettings.json de Conexão com o PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");

// Configura a Injeção de Dependência para o EF Core usar o PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// Serviços necessários para o Swagger funcionar
builder.Services.AddSwaggerGen();


// Configuração do HttpClient para API 1
builder.Services.AddHttpClient<IdGeneratorClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["IdGeneratorUrl"] ?? "http://localhost:4849");
});

// --- Configuração de Autenticação JWT ---
var jwtKey = builder.Configuration.GetSection("JWT:Key").Value;

// Garante que se a chave for nula ou muito curta, a aplicação use uma string limpa padrão
if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
{
    // Chave forte estática sem caracteres especiais complexos para evitar erros de encode
    jwtKey = "CHAVESUPERSECRETADE32CARACTERESPARATESTE";
}

var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

// Configura a autenticação JWT
builder.Services.AddJwtAuthentication(jwtKey);




// Configura a autorização
builder.Services.AddAuthorization();

// Adiciona os serviços para a Minimal API mapear endpoints
builder.Services.AddEndpointsApiExplorer();

// Configura o Rate Limiting por IP de origem
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    // essa configuração define que cada IP pode fazer no máximo 10 requisições a cada 10 segundos. Se passar disso, recebe 429 Too Many Requests.
    options.AddPolicy("IpLimitPolicy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,  // Permite que o contador de requisições seja reiniciado automaticamente após o período definido
                PermitLimit = 10, // Máximo de 10 requisições...
                Window = TimeSpan.FromSeconds(10), // ...a cada 10 segundos
                QueueLimit = 0
            }));
});


// Serviços necessários para o Swagger funcionar com suporte a JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Info Host API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http, // Alterado para Http
        Scheme = "bearer", // Em minúsculo funciona melhor em algumas versões
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira apenas o seu token JWT abaixo:"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Ative o middleware do Rate Limiter (coloque antes de mapear as rotas)
app.UseRateLimiter();


//app.UseHttpsRedirection(); // Redireciona automaticamente para HTTPS
// Ativa a autenticação e autorização Obs. que a ordem importa: UseAuthentication deve vir antes de UseAuthorization
app.UseAuthentication();
app.UseAuthorization();



// Executa Migrations Automaticamente
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    db.Database.EnsureCreated();
//}

// Ativa o Swagger para documentação da API
app.UseSwagger();

// Configurações do Swagger no ambiente de Desenvolvimento
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Info Host API v1");
        // Se quiser que o Swagger abra digitando apenas http://localhost:8585/, deixe a linha abaixo.
        // Se preferir acessar por http://localhost:8585/swagger, comente a linha abaixo com //
        c.RoutePrefix = string.Empty;
    });
//}
















// -- ROTA DE REGISTRO DE USUÁRIO
app.MapPost("/api/v1/auth/register", async (RegisterRequest req, IUrlService service) =>
{
    var result = await service.PostRegisterUserAsync(req);
    return result;

}).WithSummary("Register")
.WithDescription("Registra um novo usuário e retorna um status de sucesso.");


// -- ROTA DE LOGIN DE USUÁRIO
app.MapPost("/api/v1/auth/login", async (LoginRequest req, IUrlService service) =>
{
    var result = await service.PostAuthenticationUserAsync(req);
    return result; ;

}).WithSummary("Login")
.WithDescription("Autentica o usuário e retorna um token JWT.");



// --  ROTA DE CRIAÇÃO DE URL ENCURTADA
app.MapPost("/api/v1/urls", async ( CreateUrlRequest req, IUrlService service,   ClaimsPrincipal userClaims) =>
{
    // Recupera o ID do usuário logado a partir das claims do token JWT
    var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    // Se não houver ID de usuário, retorna 401 Unauthorized
    if (string.IsNullOrEmpty(userId))
        return Results.Unauthorized();


    //Registra a URL encurtada para o usuário logado
    var result = await service.RegisterUrlAsync(req.Url, userId);

    return result;

}).RequireAuthorization().RequireRateLimiting("IpLimitPolicy");



// -- ROTA DE LISTAGEM DE URLS ENCURTADAS COM PAGINAÇÃO
app.MapGet("/api/v1/urls", async (ClaimsPrincipal userClaims, IUrlService service, int page = 1, int limit = 10) =>
{
    // Recupera o ID do usuário logado a partir das claims do token JWT
    var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    // Se não houver ID de usuário, retorna 401 Unauthorized
    if (string.IsNullOrEmpty(userId))
        return Results.Unauthorized();

    var data = await service.ObterPageUrlPorUserIdAsync(userId, page, limit);
    return Results.Ok(data);


}).WithSummary("Lista URLs paginadas")
.WithDescription("Retorna uma lista paginada de URLs do usuário logado.").RequireAuthorization().RequireRateLimiting("IpLimitPolicy");

      



// ROTA DE EXCLUSÃO DE URL ENCURTADA
app.MapDelete("/api/v1/urls/{idOfuscado}", async (string idOfuscado, AppDbContext db, ClaimsPrincipal userClaims) =>
{
    try
    {
        // Recupera o ID do usuário logado a partir das claims do token JWT
        var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Se não houver ID de usuário, retorna 401 Unauthorized
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        // Busca a URL pelo ID ofuscado no banco de dados
        var url = await db.Urls.FirstOrDefaultAsync(u => u.IdOfuscado == idOfuscado);
        if (url == null) return Results.NotFound();

        if (url.UserId != userId) return Results.Forbid(); // 403 se não for dono do link

        // Remove a URL do banco de dados e salva as alterações
        db.Urls.Remove(url);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }
    catch
    {
        return Results.StatusCode(500); // Internal Server Error
    }

}).WithSummary("Exclui uma URL encurtada")
.WithDescription("Exclui uma URL encurtada do usuário logado.").RequireAuthorization().RequireRateLimiting("IpLimitPolicy");



// ROTA DE REDIRECIONAMENTO
app.MapGet("/{idOfuscado}", async (string idOfuscado, AppDbContext db) =>
{
    // A query abaixo utiliza o Unique Index definido no Context do EF Core.
    // O banco de dados buscará apenas no índice de forma extremamente rápida, sem "Key Lookup".
    var urlData = await db.Urls
        .Where(u => u.IdOfuscado == idOfuscado && u.IsActive)
        .Select(u => new { u.Id, u.OriginalUrl, u.ExpiresAt })
        .FirstOrDefaultAsync();

    // Se não encontrar o registro, retorna 404 Not Found
    if (urlData == null) return Results.NotFound();

    // Valida expiração de data se configurado
    if (urlData.ExpiresAt.HasValue && urlData.ExpiresAt.Value < DateTimeOffset.UtcNow)
        return Results.NotFound();


    // Incrementa estatísticas em segundo plano para não travar a resposta de redirecionamento
    _ = Task.Run(async () =>
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var record = await context.Urls.FindAsync(urlData.Id);
            if (record != null)
            {
                record.ClickCount++;
                record.LastAccessedAt = DateTimeOffset.UtcNow;
                await context.SaveChangesAsync();
            }
        }
        catch { /* Silencia erros de background thread para não impactar o usuário */ }
    });

    return Results.Redirect(urlData.OriginalUrl, permanent: false); // Redirecionamento 302 Found
}).WithSummary("Redireciona para a URL original")
.WithDescription("Redireciona para a URL original correspondente ao ID ofuscado fornecido.");



app.Run();



