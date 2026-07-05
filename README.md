# Task Manager API

API de gerenciamento de tarefas construída como projeto de portfólio para demonstrar
domínio de **.NET idiomático**. Um usuário organiza **projetos**, e cada projeto tem
suas **tarefas** (com status, prioridade e prazo).

> Projeto em construção por fases. Esta é a documentação acumulada; cada fase amplia o escopo.

## Stack

- **Back-end:** ASP.NET Core Web API — C#, .NET 10 (LTS)
- **Persistência:** Entity Framework Core + PostgreSQL (provider Npgsql)
- **Documentação/testes manuais:** Swagger (Swashbuckle)
- **Front-end:** React + TypeScript com Vite _(Fase 4)_
- **Autenticação:** JWT com ASP.NET Core Identity _(Fase 2)_

## Estrutura do monorepo

```
TaskManager/
├── backend/
│   ├── TaskManager.slnx
│   └── src/TaskManager.Api/
│       ├── Controllers/     endpoints HTTP (camada fina)
│       ├── Services/        regras de negócio
│       ├── Data/            AppDbContext
│       ├── Entities/        entidades de domínio (Project, TaskItem, enums)
│       ├── Dtos/            records de request/response
│       ├── Common/          exceções de domínio + handler global (Problem Details)
│       └── Migrations/      migrations do EF Core
├── frontend/                reservado para a Fase 4
├── docker-compose.yml       PostgreSQL para desenvolvimento
└── README.md
```

## Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/)
- [Docker](https://www.docker.com/) (para subir o PostgreSQL)
- Ferramenta de linha de comando do EF Core:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

## Como rodar (desenvolvimento)

### 1. Subir o banco

Da raiz do repositório:

```bash
docker compose up -d
```

Isso sobe um PostgreSQL na porta **5433** do host (mapeada para a 5432 do container,
para não conflitar com um Postgres eventualmente já rodando na 5432).

### 2. Configurar a connection string (User Secrets)

A connection string é lida da configuração pela chave `ConnectionStrings:DefaultConnection`.
Em desenvolvimento ela fica nos **User Secrets** (fora do controle de versão):

```bash
cd backend
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Host=localhost;Port=5433;Database=taskmanager;Username=taskmanager;Password=taskmanager" \
  --project src/TaskManager.Api
```

### 3. Aplicar as migrations

Em ambiente de desenvolvimento a API aplica as migrations pendentes automaticamente no
startup. Se quiser aplicar manualmente:

```bash
cd backend
dotnet ef database update --project src/TaskManager.Api
```

### 4. Rodar a API

```bash
cd backend
dotnet run --project src/TaskManager.Api --launch-profile http
```

- API: `http://localhost:5023`
- Swagger UI: `http://localhost:5023/swagger`

## Endpoints (Fase 1)

Nesta fase os endpoints ainda **não** exigem autenticação. Os projetos são associados a um
usuário placeholder fixo; a proteção por JWT e o escopo real por usuário chegam na Fase 2.

### Projects

| Método | Rota                  | Descrição                    |
|--------|-----------------------|------------------------------|
| POST   | `/api/projects`       | Cria um projeto              |
| GET    | `/api/projects`       | Lista os projetos            |
| GET    | `/api/projects/{id}`  | Detalha um projeto           |
| DELETE | `/api/projects/{id}`  | Remove um projeto (cascade)  |

### Tasks (escopadas ao projeto)

| Método | Rota                                       | Descrição                              |
|--------|--------------------------------------------|----------------------------------------|
| POST   | `/api/projects/{projectId}/tasks`          | Cria uma task no projeto               |
| GET    | `/api/projects/{projectId}/tasks`          | Lista as tasks (filtros opcionais)     |
| PUT    | `/api/projects/{projectId}/tasks/{id}`     | Atualiza uma task                      |
| DELETE | `/api/projects/{projectId}/tasks/{id}`     | Remove uma task                        |

Filtros opcionais no GET de tasks via query string:
`?status=Todo|InProgress|Done` e `?priority=Low|Medium|High`.

## Regras de negócio já implementadas

- Deletar um projeto apaga suas tasks em **cascade** (garantido pela FK no banco).
- `DueDate` no passado na criação de uma task é rejeitado com **400**.
- Erros seguem o formato **Problem Details (RFC 7807)**.

## Roadmap

- **Fase 1 (atual):** scaffold, EF Core + PostgreSQL, entidades, migrations, CRUD de Project e Task.
- **Fase 2:** ASP.NET Core Identity + JWT (access + refresh), proteção dos endpoints e escopo por usuário.
- **Fase 3:** testes com xUnit + Moq nos Services.
- **Fase 4:** front-end React + TypeScript (Vite + React Query).
