# Task Manager API

A simple Task Manager REST API built with ASP.NET Core, Dapper, and PostgreSQL — designed as a learning project for CI/CD with GitHub Actions, Docker, and Git workflows.

## Architecture

```
TaskManager.Domain        → Entities, Enums (no dependencies)
TaskManager.Application   → CQRS handlers, DTOs, validators (MediatR + FluentValidation)
TaskManager.Infrastructure → Dapper repository (PostgreSQL)
TaskManager.Api           → Controllers, middleware, Docker
TaskManager.Tests         → Unit tests (xUnit + Moq + FluentAssertions)
```

**Patterns:** Clean Architecture, CQRS with MediatR, Repository pattern, Pipeline behaviors for cross-cutting concerns.

## Tech Stack

- **ASP.NET Core 8** — REST API
- **Dapper** — Micro ORM for SQL queries
- **PostgreSQL 16** — Database (via Docker)
- **MediatR** — CQRS command/query dispatching
- **FluentValidation** — Request validation
- **Serilog** — Structured logging
- **xUnit + Moq + FluentAssertions** — Unit testing
- **Docker / Docker Compose** — Containerization
- **GitHub Actions** — CI/CD pipeline

## Quick Start

### Prerequisites
- .NET 8 SDK
- Docker & Docker Compose

### Run with Docker
```bash
docker-compose up -d
```
API available at `http://localhost:8080`
Swagger UI at `http://localhost:8080/swagger`

### Run locally (development)
```bash
# Store connection string in User Secrets (one-time setup)
cd TaskManager.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=taskmanager;Username=taskmanager;Password=taskmanager123"

# Start PostgreSQL
docker-compose up -d postgres

# Run API
dotnet run --project TaskManager.Api
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/tasks` | List all tasks (supports `?status=&priority=&page=&pageSize=`) |
| GET | `/api/tasks/{id}` | Get task by ID |
| POST | `/api/tasks` | Create new task |
| PUT | `/api/tasks/{id}` | Update existing task |
| DELETE | `/api/tasks/{id}` | Delete task |

### Task Model
```json
{
  "id": "guid",
  "title": "string",
  "description": "string",
  "status": "Pending | InProgress | Completed | Cancelled",
  "priority": "Low | Medium | High | Critical",
  "dueDate": "datetime | null",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

## Testing

```bash
dotnet test
```

## CI/CD

GitHub Actions workflow (`.github/workflows/ci-cd.yml`) runs on push to `main`/`develop`:
1. Restore → Build → Test
2. Docker image build
3. Container smoke test

## User Secrets (Local Dev)

Connection strings are stored in .NET User Secrets (never committed). The `appsettings.json` files contain no secrets.

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-connection-string>" --project TaskManager.Api
```
