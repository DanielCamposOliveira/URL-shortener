# 1. Imagem de Runtime do .NET (Linux)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

# 2. Imagem do SDK para Compilar o projeto
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia o arquivo .csproj e restaura as dependências
COPY ["API-Data.csproj", "./"]
RUN dotnet restore "API-Data.csproj"

# Copia todo o resto do código e compila
COPY . .
RUN dotnet build "API-Data.csproj" -c Release -o /app/build

# 3. Publica os arquivos para execução
FROM build AS publish
RUN dotnet publish "API-Data.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Imagem Final de Execução
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API-Data.dll"]