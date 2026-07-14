using API_Data.src.Data;
using API_Data.src.DTOs;
using API_Data.src.Models;
using API_Data.src.Repository;
using API_Data.src.Services;
using API_Data.src.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // tenho que mudar para true quando colocar em produção
    options.SaveToken = true;  // Salva o token no contexto da requisição

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, // Valida a chave de assinatura do token
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes), // Define a chave de assinatura usada para validar o token
        ValidateIssuer = false, // Não valida o emissor
        ValidateAudience = false, // Não valida o público
        ClockSkew = TimeSpan.Zero // isso significa que não vai aceita  tolerancia de tokem espirado, ex. 5min de atraso
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var authorizationHeader = context.Request.Headers["Authorization"].ToString();
            Console.WriteLine($"[JWT] Token recebido no Header: {authorizationHeader}");
            return Task.CompletedTask;
        },

        OnTokenValidated = context =>
        {
            Console.WriteLine("[JWT] Token validado com sucesso!");
            return Task.CompletedTask;
        },

        OnAuthenticationFailed = context =>
        {
            // Isso vai printar o motivo EXATO no console da sua aplicação quando der erro
            Console.WriteLine($"[JWT] Falha na validação do Token: {context.Exception.Message}");
            return Task.CompletedTask;
        },

        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var resultado = new { message = "Não autorizado. Você precisa enviar um token JWT válido no Header." };
            return context.Response.WriteAsJsonAsync(resultado);
        },

        OnForbidden = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            var resultado = new { message = "Você não tem permissão para acessar este recurso." };
            return context.Response.WriteAsJsonAsync(resultado);
        }
    };

});

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


// Método auxiliar para gerar tokens JWT
// Método auxiliar para gerar tokens JWT (usando o Id em string do usuário)
//string GenerateJwtToken(User user)
//{
//    var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
//    var tokenDescriptor = new SecurityTokenDescriptor
//    {
//        // NameIdentifier passa a armazenar o ID do usuário como string (GUID)
//        Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id) }),

//        // Expiração de 2 horas
//        Expires = DateTime.UtcNow.AddHours(2),

//        // Define a chave de assinatura e o algoritmo
//        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
//    };

//    var token = tokenHandler.CreateToken(tokenDescriptor);
//    return tokenHandler.WriteToken(token);
//}

// Este método extrai o ID do usuário a partir das claims do token JWT retornando como string
string GetUserId(ClaimsPrincipal userPrincipal) =>
    userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
















// ROTA DE REGISTRO DE USUÁRIO
app.MapPost("/api/v1/auth/register", async (RegisterRequest req, AppDbContext db) =>
{
    try
    {
        if (await db.Users.AnyAsync(u => u.Email == req.Email))
            return Results.BadRequest(new { message = "E-mail já cadastrado." });

        var user = new User
        {
            Name = req.Name,
            Email = req.Email,
            PasswordHash = PasswordHasher.HashPassword(req.Password)
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return Results.StatusCode(201); // 201 Created
    }
    catch
    {
        return Results.StatusCode(500); // Internal Server Error
    }

}).WithSummary("Register")
.WithDescription("Registra um novo usuário e retorna um status de sucesso.");



// ROTA DE LOGIN DE USUÁRIO
//app.MapPost("/api/v1/auth/login", async (LoginRequest req, AppDbContext db, IConfiguration config) =>
//{
//    try
//    {
//        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
//        if (user == null || !PasswordHasher.VerifyPassword(req.Password, user.PasswordHash))
//            return Results.Unauthorized();

//        // Gera o token JWT para o usuário logado 
//        var token = GenerateJwtToken(user);

//        return Results.Ok(new UserDtos.AuthResponse(token)); // Retorna o token em caso de sucesso
//    }
//    catch
//    {
//        return Results.StatusCode(500); // Internal Server Error
//    }

//}).WithSummary("Login")
//.WithDescription("Autentica o usuário e retorna um token JWT.");

app.MapPost("/api/v1/auth/login", async (LoginRequest req, IUrlService service, IConfiguration config) =>
{  
    try
    {
        // Busca o usuário no banco de dados pelo e-mail fornecido
        var user = await service.ObterUsuarioPorEmailAsync(req);
        if (user == null)
            return Results.Unauthorized();

        return Results.Ok(new UserDtos.AuthResponse(user.Token)); // Retorna o token em caso de sucesso
    }
    catch
    {
        return Results.StatusCode(500); // Internal Server Error
    }

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

    try
    {
        var url = await service.CriarUrlAsync(req.Url, userId);

        return Results.Created($"/api/v1/urls/{url.IdOfuscado}", new
        {
            idOfuscado = url.IdOfuscado,
            urlEncurtada = $"https://meusite.com/{url.IdOfuscado}"
        });
    }
    catch
    {
        return Results.StatusCode(500);
    }
}).RequireAuthorization().RequireRateLimiting("IpLimitPolicy");




// ROTA DE LISTAGEM DE URLS ENCURTADAS COM PAGINAÇÃO
app.MapGet("/api/v1/urls", async (ClaimsPrincipal userPrincipal, AppDbContext db, int page = 1, int limit = 10) =>
{
    try
    {
        string userId = GetUserId(userPrincipal);
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        // Garantir paginação mínima válida
        if (page < 1) page = 1;
        if (limit < 1 || limit > 50) limit = 10;


        // Consulta base filtrando pelo usuário logado
        var query = db.Urls.Where(u => u.UserId == userId);
        // Conta o total de registros para paginação
        var total = await query.CountAsync();

        var data = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(u => new
            {
                IsActive  = u.IsActive,
                ClickCount = u.ClickCount,
                ExpiresAt = u.ExpiresAt,
                LastAccessedAt = u.LastAccessedAt,
                IdOfuscado = u.IdOfuscado,
                OriginalUrl = u.OriginalUrl
              
              
            })
            .ToListAsync();

        return Results.Ok(new
        {
            data,
            page,
            limit,
            total
        });
    }
    catch
    {
        return Results.StatusCode(500); // Internal Server Error
    }
    
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



