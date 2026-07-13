# SheikGenAIAPI (AgentAIPlatform)

A multi-service .NET microservices platform combining enterprise backend architecture with Generative AI — built as a hands-on portfolio project to explore Clean Architecture, CQRS, and Semantic Kernel-based AI agents in a realistic HR workflow domain.

## What it does

An HR assistant system where employees interact with an AI agent in natural language ("apply leave from Dec 20–24 for vacation") and the agent calls backend HR APIs on their behalf — authenticated, auditable, and routed through a central gateway.

## Architecture

```
                    ┌─────────────────────┐
                    │   Ocelot Gateway     │  ← single entry point, JWT-secured routing
                    └──────────┬───────────┘
                               │
        ┌──────────────────────┼──────────────────────┐
        │                      │                       │
┌───────▼────────┐    ┌────────▼────────┐    ┌─────────▼────────┐
│   Auth.API      │    │   Agent.API      │    │    HR.API         │
│  JWT + Refresh   │    │  Semantic Kernel │───▶│  CQRS + MediatR   │
│  Token Rotation  │    │  LeavePlugin     │    │  EF Core           │
│  CQRS + MediatR  │    │  OnboardingPlugin│    │  Employees,        │
│                  │    │  Multi-provider  │    │  LeaveRequests,    │
│                  │    │  LLM routing     │    │  OnboardingTasks   │
└──────────────────┘    └──────────────────┘    └────────────────────┘
```

Each service owns its own database and is independently deployable via Docker.

## Services

### Auth.API
- JWT authentication with refresh token rotation
- Role-based registration and authorization (`AdminOnlyFilter`)
- Clean Architecture: `Auth.Domain` → `Auth.Application` (CQRS/MediatR + FluentValidation) → `Auth.Infrastructure` (EF Core) → `Auth.API`
- Endpoints: `POST /register`, `POST /login`, `POST /refresh`, `GET /users` (admin only), `POST /logout`

### HR.API
- Domain model: `Employee`, `LeaveBalance`, `LeaveRequest`, `OnboardingTask` — with self-referencing manager hierarchy on `Employee`
- Same layered structure as Auth.API (Domain → Application → Infrastructure → API)
- Endpoints:
  - `POST /employees/register`, `GET /employees/{id}`
  - `POST /leaverequests`, `PUT /leaverequests/approve`, `GET /leaverequests/{employeeId}`
  - `GET /onboardingtasks/{employeeId}`, `POST /onboardingtasks/seed/{employeeId}`, `PUT /onboardingtasks/complete`

### Agent.API
- Orchestrates conversations using **Microsoft Semantic Kernel**
- Stateful `ChatHistory` per employee session, with `AutoInvokeKernelFunctions` for tool calling
- Two plugins forward the caller's JWT to HR.API so the AI agent acts with the same permissions as the user:
  - `LeavePlugin` — apply for leave, check leave requests
  - `OnboardingPlugin` — view and complete onboarding tasks
- **Provider-agnostic LLM routing**: model, API key, and provider (NVIDIA NIM / OpenAI) are supplied per-request in the `ChatRequest` body — no redeployment needed to switch providers
- Endpoint: `POST /agent/chat`

### Ocelot API Gateway
- Single entry point for all downstream services
- JWT bearer validation at the gateway layer with `AllowedScopes` per route
- Centralized routing config in `ocelot.json`

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 10 |
| API Style | ASP.NET Core Web API, REST, API Versioning |
| Architecture | Clean Architecture, CQRS + MediatR, Repository/Unit of Work |
| Validation | FluentValidation (MediatR pipeline behavior) |
| Data | Entity Framework Core, SQL Server |
| Auth | JWT Bearer, Refresh Token Rotation |
| AI | Microsoft Semantic Kernel, NVIDIA NIM, OpenAI (multi-provider) |
| Gateway | Ocelot + Polly (resilience) |
| Containerization | Docker, Docker Compose |

## Getting Started

### Prerequisites
- .NET 10 SDK
- Docker Desktop
- SQL Server (or use the Dockerized instance)

### Run locally

```bash
# Clone
git clone https://github.com/SheikMydeenMuthu/SheikGenAIAPI.git
cd SheikGenAIAPI

# Set required environment variables (see compose.yaml)
# DB_CONNECTION, JWT_KEY, JWT_ISSUER, JWT_AUDIENCE

# Run all services
docker compose up --build
```

Each service also runs independently via `dotnet run` from its own project folder for local debugging — see `compose.debug.yaml` for the debug configuration.

### Configuration

All secrets (`Jwt:Key`, connection strings) are externalized via environment variables — `appsettings.json` files ship with empty values by design. Set them via a local `.env` file (gitignored) or your shell environment before running.

## Roadmap

- [ ] `GraphPlugin` — Microsoft Graph integration for Outlook Calendar and Teams notifications
- [ ] Gateway hardening — restrict CORS policy (currently permissive for local dev)
- [ ] Centralized structured logging (Serilog) across all services
- [ ] Redis caching layer for frequently accessed HR data

## Author

**Sheik Mydeen Muthu A** — Lead .NET Backend Developer
[LinkedIn](https://www.linkedin.com/in/sheikmydeenmuthu) · [GitHub](https://github.com/SheikMydeenMuthu)
