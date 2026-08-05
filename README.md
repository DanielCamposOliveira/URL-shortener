

# 📝 URL shortener

Uma API RESTful robusta desenvolvida para o gerenciamento de **URLs** de usuários, trazendo recursos avançados de autenticação, segurança e controle de dados.

---
## 🎯 Objetivo do Projeto

O propósito desta API é ir além do CRUD tradicional. O sistema garante o isolamento dos dados por usuário, com validação de payloads, paginação inteligente de recursos e segurança no tráfego de informações através de tokens de autenticação.

<img width="1415" height="1138" alt="image" src="https://github.com/user-attachments/assets/6315f563-c605-4cef-9415-2b23d4321fd2" />

## 🛠️ Tecnologias Utilizadas

- **Back-end:** .NET 9 / ASP.NET com Minimal API
- **Front-end:** .NET 9 / ASP.NET MVC
- **Persistência & ORM:** Entity Framework Core
- **Banco de Dados:** PostgreSQL
- **Segurança & Autenticação:** JWT (JSON Web Tokens) & BCrypt para Hash de Senhas
- **Documentação:** Swagger (OpenAPI)

---

## ⚙️ Funcionalidades Principais
- **Autenticação & Segurança:**
    - Registro de novos usuários com senha criptografada.
    - Geração de Tokens JWT expiráveis no Login.
    - Bloqueio e isolamento de rotas por usuarios.
- **Gerenciamento de URLs:**
    - CRUD completo de tarefas com proteção de escopo por Token.
- **Gerenciamento de Usuários:**
    - Criação e gerenciamento de usuários.
- **Performance:**
    - Paginação dinâmica e filtros via Query Strings nos endpoints de listagem.

---

## Como Executar o Projeto Localmente

Pré-requisitos
- .NET SDK 9
- PostgreSQL

**Clonar o Repositório:**

```shell
git clone https://github.com/DanielCamposOliveira/URL-shortener.git
```

**Configurar as Variáveis de Ambiente:** Atualize a Connection String e a Secret Key do JWT no arquivo appsettings.json

**Rodar as Migrations do Entity Framework:**

```shell
dotnet ef database update
```

**Executar a Aplicação:**

```shell
dotnet run
```



---

# Documentação Backend

### API 1: Geração de Identificadores (ID Generator)

API isolada de alta performance responsável exclusivamente pela geração de identificadores únicos através do algoritmo Snowflake, reduzindo a contenção no banco de dados.
#### Endpoint: /api/v1/identificadores

**Método:** GET 
**Descrição:** Gera um ID único e sequencial baseado na arquitetura Snowflake usando Base62 ofuscada por senha.

**Responses (JSON):** **Code:** 200 OK
```json
{
  "idNumerico": 13573682176,
  "idOfuscado": "Bgabx77A86x",
  "maquinaOrigem": 1
}
```

### API 2: Armazenamento e Gerenciamento de Links

Minimal API responsável pela persistência, autenticação de usuários e resolução de redirecionamentos.

Pacotes NuGet Necessários
```cmd
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package Microsoft.IdentityModel.Tokens

```
### 🗄️ Modelagem de Dados

O banco de dados foi projetado focando em alta performance de leitura (redirecionamento) e consistência de dados através do ORM **Entity Framework Core**.
#### Tabela: urls

| Campo                | Tipo (PostgreSQL)        | Restrições / Índices      | Descrição                                                                        |
| :------------------- | :----------------------- | :------------------------ | :------------------------------------------------------------------------------- |
| **id**               | BIGINT                   | PRIMARY KEY               | ID único, numérico e sequencial gerado pelo algoritmo Snowflake.                 |
| **id_ofuscado**      | VARCHAR(15)              | UNIQUE INDEX              | Representação Base62 do id usada para busca e mapeamento da rota pública.        |
| **original_url**     | VARCHAR(2048)            | NOT NULL                  | URL longa de destino do usuário. Limite padrão para compatibilidade de browsers. |
| **created_at**       | TIMESTAMP WITH TIME ZONE | NOT NULL (Default: NOW()) | Data e hora de criação do registro.                                              |
| **expires_at**       | TIMESTAMP WITH TIME ZONE | NULLABLE                  | Data limite de validade do link. Se nulo, o link é permanente.                   |
| **click_count**      | INTEGER                  | NOT NULL (Default: 0)     | Contador incremental acumulado de acessos ao link.                               |
| **last_accessed_at** | TIMESTAMP WITH TIME ZONE | NULLABLE                  | Data e hora do último redirecionamento bem-sucedido.                             |
| **is_active**        | BOOLEAN                  | NOT NULL (Default: true)  | Flag para desativação/bloqueio lógico do link (ex: por abuso ou denúncia).       |


Criamos um índice não-clustered (Unique Index) na coluna id_ofuscado incluindo (*Included Columns*) a coluna original_url. Dessa forma, no momento do redirecionamento, o banco realiza um *Index Seek* extremamente rápido sem necessidade de consultar a tabela física (evitando *Key Lookup*), reduzindo drasticamente o uso de I/O do banco de dados.

#### 1. Autenticação de Usuários

#### Endpoint:  /api/v1/auth/register
**Método:** POST
**Descrição:** Registra um novo usuário com senha criptografada e retorna o token JWT de acesso.
**Request Body:**
```json
{
	"name": "string",
	"email": "string",
	"password": "string"
}
```

**Responses:**
- **Code:** 201 Created


#### Endpoint:  /api/v1/auth/login
**Método:** Post
**Descrição:** Autentica o usuário e retorna um token JWT.
 **Request Body:**
```json
{
	"email": "string",
	"password": "string"
}
```

**Responses:**
- **Code:** 200 OK
```json
{
  "token": "1lajptrwvdpgkgçfQiOiIiwibmJmIjoxNzgz..."
}
```

#### 2. Rotas Administrator
#### Endpoint:  /api/v1/user/{UserID}
**Método:**  PATCH
**Descrição:** Rota de Desativa ou Ativa usuário

**Responses:**
- **Code:** 204 No Content
- **Errors:** 401 Unauthorized,

##### Endpoint:  /api/v1/user/{UserID}
**Método:**  DELETE
**Descrição:** Rota de Deletar usuário

**Responses:**
- **Code:** 204 No Content
- **Errors:** 401 Unauthorized,

#### 3. Rota de URLs

##### Endpoint:  /api/v1/urls
**Método:**  POST
**Descrição:** Encurta uma URL longa. _(Nota de arquitetura: A API recebe a URL do Front, consome internamente a API 1 para obter o ID Snowflake/Base62 e persiste os dados)

**Request Body:**
```json
{
	"url": "https://docs.netgate.com/pfsense/en/latest/packages/cache-proxy/squidguard.html"
}
```

**Responses:**
- **Code:** 201 Created
```json
{
  "idOfuscado": "Bgabx77A86x",
  "urlEncurtada": "https://meusite.com/Bgabx77A86x"
}
```

##### Endpoint: /api/v1/urls
**Método:**  GET
**Descrição:** Retorna as URLs cadastradas pelo usuário autenticado de forma paginada.
**Query Parameters:** `page` (padrão: 1), `limit` (padrão: 10)
```txt
http://localhost:8585/todos?page=1&limit=10
```

**Responses:**
- **Code:** 200 OK
```json
{
  "data": [
    {
      "idOriginal": 13573682176,
      "idOfuscado": "Bgabx77A86x",
      "urlDestino": "https://docs.netgate.com/pfsense/en/latest/packages/cache-proxy/squidguard.html",
      "urlEncurtada": "https://meusite.com/Bgabx77A86x"
    }
  ],
  "page": 1,
  "limit": 10,
  "total": 1
}
```

#### Endpoint:  /api/v1/urls/{idOfuscado}
**Método:**  DELETE
**Descrição:** Remove uma URL encurtada do banco de dados.

**Responses:**
- **Code:** 204 No Content
- **Errors:** 401 Unauthorized, 403 Forbidden (se a URL não pertencer ao usuário logado), 404 Not Found.


#### 3. Mecanismo de Redirecionamento (Público)

#### Endpoint: /{idOfuscado}
**Método:**  GET
**Descrição:** Rota otimizada utilizada para realizar o redirecionamento para a URL de destino.
 
**Responses:**
- **Code:** 301 Moved Permanently ou 302 Found

- **Headers:**
```Plaintext
Location: https://docs.netgate.com/pfsense/en/latest/packages/cache-proxy/squidguard.html
```

- **Errors:** 404 Not Found (caso o idOfuscado não exista no banco).

























