using HashidsNet;
using IdGeneratorService.src.service;

var builder = WebApplication.CreateBuilder(args);

// 1. Prepara o servidor injetando threads suficientes logo na inicialização
// Isso evita que o .NET se engasgue no primeiro segundo do teste de carga
ThreadPool.SetMinThreads(2000, 2000);

// 2. Avisa ao Kestrel que ele vai receber tráfego pesado e aumenta a fila de espera
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Aumenta o limite de conexões simultâneas (o padrão geralmente é menor que 2000)
    serverOptions.Limits.MaxConcurrentConnections = 3000;
    serverOptions.Limits.MaxConcurrentUpgradedConnections = 3000;
});


// 1. Pega o ID da máquina. Se não achar a variável de ambiente "MACHINE_ID", usa 1 como padrão.
int machineId = builder.Configuration.GetValue<int>("config:MACHINE_ID", 1);


builder.Services.AddSingleton(new SnowflakeGenerator(machineId));
builder.Services.AddSingleton(new Hashids(builder.Configuration.GetSection("config:SaltSecret").Value, 7));



var app = builder.Build();


// Rota oficial para gerar IDs. Retorna tanto o ID numérico quanto o ID ofuscado.
app.MapGet("/gerar-id", (SnowflakeGenerator gerador, Hashids hashids) =>
{
    // Gera o número Snowflake à prova de colisões
    long numericId = gerador.GerarProximoId();

    // Ofusca o número usando o seu Hashids
    string hashId = hashids.EncodeLong(numericId);

    return Results.Ok(new
    {
        IdNumerico = numericId,
        IdOfuscado = hashId,
        MaquinaOrigem = machineId
    });
});






app.Run();
