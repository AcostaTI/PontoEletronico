# Ponto Eletrônico

Sistema de registro de ponto, com backend em ASP.NET Core e frontend em Angular.

O usuário se cadastra, faz login e bate o ponto. O tipo da batida (ENTRADA ou SAÍDA) é decidido pela API, alternando a partir do último registro do dia — não existe botão separado de entrada e saída. As batidas podem ser consultadas no dia corrente ou no espelho mensal.

## Funcionalidades

- Cadastro e login de usuários com autenticação JWT
- Registro de ponto com tipo (entrada/saída) inferido automaticamente
- Consulta do ponto do dia e do mês
- Visualização do espelho mensal com o total de horas trabalhadas
- Documentação Swagger para a API

## Componentes

| Componente | Tecnologia | Endereço local |
| --- | --- | --- |
| Backend | ASP.NET Core 6 + EF Core + SQL Server | https://localhost:7239 |
| Frontend | Angular 22 | http://localhost:4200 |

O backend usa Entity Framework Core 6, ASP.NET Core Identity (cadastro e regras de senha), JWT Bearer, AutoMapper e Swashbuckle (Swagger). Os testes são em xUnit, com Moq e FluentAssertions.

O frontend usa componentes standalone e signals (sem NgModules), TypeScript 6 e SCSS puro — não há biblioteca de UI nem de estado (NgRx e afins). O design system é próprio, definido com CSS custom properties em [frontend/src/styles.scss](frontend/src/styles.scss); o estado vive em `signal()` dentro do `AuthService` e dos próprios componentes.

## Requisitos

- .NET SDK 6.0
- SQL Server Express ou Developer
- Node.js 20+ e npm 10+
- dotnet-ef 6.x, para aplicar as migrations:

```bash
dotnet tool install --global dotnet-ef --version 6.*
```

A solution está no formato `.slnx` ([api/PontoEletronicoAPI.slnx](api/PontoEletronicoAPI.slnx)), que só abre em Visual Studio 2022 17.10+ ou em SDKs mais recentes. Os comandos documentados apontam direto para os `.csproj`, então funcionam com o SDK 6 de qualquer forma.

## Execução rápida

Suba a API primeiro, depois o frontend.

```bash
# 1. Backend — na pasta api/
cd api
dotnet restore
dotnet user-secrets set "Jwt:Key" "uma-chave-secreta-com-pelo-menos-32-caracteres" --project src/PontoEletronico.API
dotnet dev-certs https --trust
dotnet ef database update --project src/PontoEletronico.Infrastructure --startup-project src/PontoEletronico.API
dotnet run --project src/PontoEletronico.API

# 2. Frontend — em outro terminal, a partir da raiz do repositório
cd frontend
npm install
npm start
```

Acesse o sistema em http://localhost:4200. A documentação da API fica em https://localhost:7239/swagger.

Três detalhes costumam travar a primeira execução:

1. `Jwt:Key` vem **vazio** em [appsettings.json](api/src/PontoEletronico.API/appsettings.json) e a API se recusa a subir sem ele.
2. As migrations **não** são aplicadas na inicialização — sem o `dotnet ef database update`, a API sobe mas quebra na primeira consulta.
3. Sem `dotnet dev-certs https --trust`, o proxy do frontend não consegue falar com a API.

## Fluxo de uso

1. Acesse http://localhost:4200 e crie uma conta na tela de cadastro.
2. Faça login. O token JWT vale 1 hora.
3. No dashboard, clique em bater ponto. A primeira batida do dia é uma ENTRADA; a próxima, uma SAÍDA, e assim por diante.
4. Consulte as batidas do dia no dashboard, ou o mês fechado na tela de espelho.

## Estrutura do repositório

```
api/                                     # Backend .NET (solution + projetos)
├── PontoEletronicoAPI.slnx
├── src/
│   ├── PontoEletronico.Domain/          # Entidades e regras de negócio. Sem dependências.
│   │   ├── Entities/RegistroPonto.cs    # Onde vive a regra de alternância ENTRADA/SAIDA
│   │   ├── Enums/
│   │   └── Exceptions/
│   ├── PontoEletronico.Application/     # Casos de uso, DTOs, interfaces, mapeamentos
│   │   ├── RegistrosPonto/
│   │   └── Usuarios/
│   ├── PontoEletronico.Infrastructure/  # EF Core, Identity, geração do token
│   │   ├── Identity/
│   │   └── Persistence/                 # DbContext, Configurations, Migrations, Repositories
│   └── PontoEletronico.API/             # Controllers, middleware, Swagger, configuração
│       ├── Controllers/
│       ├── Middleware/                  # ExceptionHandlingMiddleware -> ProblemDetails
│       └── Json/                        # Converters de DateOnly e TimeOnly
└── tests/PontoEletronico.UnitTests/     # Testes de Domain e Application

frontend/src/app/                        # Aplicação Angular
├── core/
│   ├── auth/                            # AuthService, authGuard, authInterceptor, decodificação do JWT
│   ├── http/                            # erroInterceptor
│   ├── models/                          # Usuario, RegistroPonto
│   └── services/                        # RegistroPontoService, cálculo da jornada
├── features/
│   ├── auth/                            # Telas de login e cadastro
│   └── ponto/                           # Telas de dashboard e espelho
└── layout/shell/                        # Navbar das rotas autenticadas
```

O backend segue Clean Architecture em quatro camadas: `API → Application + Infrastructure`, `Infrastructure → Application`, `Application → Domain`. O domínio não conhece ninguém.

## Configuração do backend

### Conexão com o banco

Ajuste a conexão com o SQL Server em [appsettings.json](api/src/PontoEletronico.API/appsettings.json):

```json
"ConnectionStrings": {
  "PontoEletronicoConnection": "Server=localhost\\SQLEXPRESS;Database=PontoEletronico;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### Chave do JWT

A chave `Jwt:Key` vem **vazia** em `appsettings.json`, e a API valida essa configuração na inicialização: sem ela, o processo não sobe. Defina-a com user-secrets, que mantém o segredo fora do repositório (a partir da pasta `api/`):

```bash
dotnet user-secrets init --project src/PontoEletronico.API
dotnet user-secrets set "Jwt:Key" "uma-chave-secreta-com-pelo-menos-32-caracteres" --project src/PontoEletronico.API
```

O arquivo [appsettings.Development.json](api/src/PontoEletronico.API/appsettings.Development.json) já fornece uma chave JWT padrão para rodar local. Ela está versionada — **não use esse valor em produção**.

### Todas as configurações

| Chave | Descrição | Padrão |
| --- | --- | --- |
| `ConnectionStrings:PontoEletronicoConnection` | Conexão com o SQL Server | `Server=localhost\SQLEXPRESS;Database=PontoEletronico;...` |
| `Jwt:Key` | Chave de assinatura do token (HMAC-SHA256) | vazio — **obrigatório definir** |
| `Jwt:Issuer` | Emissor do token | `PontoEletronicoAPI` |
| `Jwt:Audience` | Destinatário do token | `PontoEletronicoAPI` |
| `Jwt:ExpiraEmHoras` | Validade do token, em horas | `1` |
| `Cors:OrigensPermitidas` | Origens liberadas no CORS | `["http://localhost:4200"]` |

Qualquer uma pode ser sobrescrita por variável de ambiente, no padrão do .NET (`:` vira `__`):

```bash
export Jwt__Key="..."
export ConnectionStrings__PontoEletronicoConnection="..."
export Cors__OrigensPermitidas__0="https://meu-front.exemplo.com"
```

## Banco de dados

As migrations **não** são aplicadas automaticamente na inicialização. Sem este passo, a API sobe mas falha na primeira consulta ao banco (a partir da pasta `api/`):

```bash
dotnet ef database update --project src/PontoEletronico.Infrastructure/PontoEletronico.Infrastructure.csproj --startup-project src/PontoEletronico.API/PontoEletronico.API.csproj
```

O banco resultante tem as tabelas padrão do ASP.NET Identity (`AspNetUsers` e companhia) mais a tabela `RegistrosPontos`.

Para criar uma nova migration depois de alterar o modelo:

```bash
dotnet ef migrations add NomeDaMigration --project src/PontoEletronico.Infrastructure/PontoEletronico.Infrastructure.csproj --startup-project src/PontoEletronico.API/PontoEletronico.API.csproj
```

## API

A API fica disponível em https://localhost:7239 e http://localhost:5129. O Swagger, em https://localhost:7239/swagger, só é exposto quando o ambiente é `Development` (o padrão do `dotnet run`). Ele já vem com o botão **Authorize** configurado: cole o token obtido no login para testar os endpoints protegidos.

### Endpoints principais

| Método | Rota | Autenticação |
| --- | --- | --- |
| POST | `/api/Usuario/cadastro` | anônima |
| POST | `/api/Usuario/login` | anônima |
| POST | `/api/RegistroPonto/RegistraPonto` | Bearer |
| GET | `/api/RegistroPonto/ObterPontoDia` | Bearer |
| GET | `/api/RegistroPonto/ObterPontoMes?ano={1-9999}&mes={1-12}` | Bearer |

`POST /api/Usuario/cadastro` recebe `username`, `email`, `password` e `rePassword` (que precisa ser igual a `password`). A senha segue as regras padrão do ASP.NET Core Identity: no mínimo 6 caracteres, com ao menos uma maiúscula, uma minúscula, um dígito e um caractere não alfanumérico.

`POST /api/Usuario/login` recebe `username` e `password`, e devolve `{ "token": "..." }`.

`POST /api/RegistroPonto/RegistraPonto` **não recebe body**. O usuário vem do token, e o tipo da batida é decidido pela API: se a última batida do dia foi uma ENTRADA, a próxima é SAÍDA, e vice-versa. A primeira batida do dia é sempre ENTRADA.

Nas respostas, `tipoRegistro` é um número: `1` = ENTRADA, `2` = SAIDA. `dataRegistro` vem como `"2026-07-13"` e `horaRegistro` como `"08:31:07"`.

### Exemplo de uso ponta a ponta

```bash
# 1. Cadastro
curl -X POST https://localhost:7239/api/Usuario/cadastro \
  -H "Content-Type: application/json" \
  -d '{
        "username": "alexandre",
        "email": "alexandre@exemplo.com",
        "password": "Senha@123",
        "rePassword": "Senha@123"
      }'
# -> { "message": "Usuário cadastrado!" }

# 2. Login (guarde o token)
TOKEN=$(curl -s -X POST https://localhost:7239/api/Usuario/login \
  -H "Content-Type: application/json" \
  -d '{ "username": "alexandre", "password": "Senha@123" }' \
  | jq -r .token)

# 3. Bater o ponto (sem body)
curl -X POST https://localhost:7239/api/RegistroPonto/RegistraPonto \
  -H "Authorization: Bearer $TOKEN"
# -> { "id": "...", "dataRegistro": "2026-07-13", "horaRegistro": "08:31:07", "tipoRegistro": 1 }

# 4. Batidas do dia
curl -H "Authorization: Bearer $TOKEN" \
  https://localhost:7239/api/RegistroPonto/ObterPontoDia

# 5. Espelho do mês
curl -H "Authorization: Bearer $TOKEN" \
  "https://localhost:7239/api/RegistroPonto/ObterPontoMes?ano=2026&mes=7"
```

### Tratamento de erros

O `ExceptionHandlingMiddleware` converte exceções em respostas `ProblemDetails` (`application/problem+json`):

- **400** para violações de regra de negócio, com a lista de mensagens na propriedade `erros`
- **500** para qualquer falha inesperada

## Autenticação

O login devolve um JWT assinado em HMAC-SHA256, válido por `Jwt:ExpiraEmHoras` (1 hora por padrão). O token deve ser enviado no header `Authorization: Bearer <token>`.

A validação usa `ClockSkew` zero — token expirado é rejeitado imediatamente, sem tolerância. O usuário é identificado pela claim `id`, que a API usa para vincular cada batida ao seu dono. Não há roles nem policies: a autorização é binária, autenticado ou não.

No frontend, o token é guardado no `localStorage`, sob a chave `ponto-eletronico.token`. O `AuthService` restaura a sessão ao iniciar a aplicação e agenda o logout automático para o momento da expiração do token. Dois interceptors cuidam do resto:

- **`authInterceptor`** injeta o header `Authorization: Bearer <token>` em todas as requisições, exceto login e cadastro.
- **`erroInterceptor`** traduz as respostas `ProblemDetails` da API em uma mensagem única de erro, e faz logout automático em caso de `401`.

## Frontend

### Rotas

- `/login`
- `/cadastro`
- `/dashboard`
- `/espelho`

`/login` e `/cadastro` são protegidas pelo `visitanteGuard` (quem já está logado é mandado para o dashboard). `/dashboard` e `/espelho` vivem dentro de um shell protegido pelo `authGuard`, que redireciona visitantes para `/login`. Todas as rotas são carregadas sob demanda com `loadComponent`.

### Comunicação com a API

As chamadas passam pelo proxy configurado em [frontend/proxy.conf.json](frontend/proxy.conf.json), que encaminha as requisições para https://localhost:7239. Isso evita problemas com CORS durante o desenvolvimento.

A URL da API vem dos arquivos de ambiente, trocados no build pelo `fileReplacements` do [frontend/angular.json](frontend/angular.json):

| Arquivo | Usado em | `apiUrl` |
| --- | --- | --- |
| [environment.development.ts](frontend/src/environments/environment.development.ts) | `npm start` | `/api` (relativo, passa pelo proxy) |
| [environment.ts](frontend/src/environments/environment.ts) | `npm run build` | `https://localhost:7239/api` |

### Build de produção

```bash
cd frontend
npm run build
```

O resultado é gerado em `dist/`. Antes de publicar, ajuste o `apiUrl` em [environment.ts](frontend/src/environments/environment.ts) para o endereço real da API — o valor versionado aponta para `localhost`.

Não existe um script de `preview`: para servir o build localmente, use um servidor estático qualquer (por exemplo, `npx http-server dist/`).

## Testes

Os testes automatizados cobrem apenas o backend (xUnit):

```bash
cd api && dotnet test
```

São testes unitários de Domain e Application, cobrindo a alternância ENTRADA/SAÍDA, os serviços de ponto e de usuário e a configuração do AutoMapper. Não há testes de integração da API nem testes no frontend.

## Problemas comuns

**A API não sobe e reclama da seção `Jwt`.** A chave `Jwt:Key` não foi definida. Volte à [chave do JWT](#chave-do-jwt).

**Erro de objeto/tabela inexistente na primeira requisição.** As migrations não foram aplicadas. Rode o `dotnet ef database update`.

**O frontend não consegue chamar a API.** Quase sempre é o certificado de desenvolvimento não confiado: rode `dotnet dev-certs https --trust` e reinicie o terminal. Se as requisições falham com erro de conexão, confirme que a API está no ar em https://localhost:7239 — o frontend não funciona sem ela.
