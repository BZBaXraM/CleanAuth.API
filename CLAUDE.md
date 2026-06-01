# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

Run from the **solution root** (`/Users/baxram/RiderProjects/CleanAuth.API`) unless noted.

```bash
# Start the database (required before running the app)
docker compose up -d

# Build the full solution
dotnet build CleanAuth.slnx

# Run the API (Swagger at https://localhost:7228/swagger)
dotnet run --project CleanAuth.API/CleanAuth.API.csproj

# EF Core migrations (DbContext is in CleanAuth.Infrastructure; API is the startup project)
dotnet ef migrations add <MigrationName> --project CleanAuth.Infrastructure --startup-project CleanAuth.API
dotnet ef database update --project CleanAuth.Infrastructure --startup-project CleanAuth.API
dotnet ef migrations remove --project CleanAuth.Infrastructure --startup-project CleanAuth.API
```

There are no automated tests in this project yet.

## Architecture

4-project Clean Architecture solution targeting .NET 10, using PostgreSQL (via Npgsql + EF Core). No ASP.NET Core Identity — everything is custom-built.

**Dependency direction (strictly inward):**
```
CleanAuth.Domain  (no external deps)
      ↑
CleanAuth.Application  (depends on Domain; no ASP.NET Core framework ref)
      ↑
CleanAuth.Infrastructure  (depends on Domain + Application)
      ↑
CleanAuth.API  (depends on Application + Infrastructure)
```

**Request flow:**
```
HTTP → ExceptionMiddleware → BlackListMiddleware → Controller → AccountService → UnitOfWork → UserRepository → AuthContext (EF Core)
```

**Key design decisions:**

- **`ResponseModel<T>` pattern** (`CleanAuth.Domain/Common/ResponseModel.cs`): All service methods return `ResponseModel` or `ResponseModel<T>` — never throw for business logic errors. Controllers call `StatusCode(result.StatusCode, result)` to propagate the code. The `IsSucceeded`/`IsFailed` properties drive branching. Static factory methods: `ResponseModel.Success(...)`, `ResponseModel.Failure(...)`.

- **Unit of Work** (`CleanAuth.Application/Repositories/IUnitOfWork.cs`, impl in `CleanAuth.Infrastructure/Repositories/UnitOfWork.cs`): `IUnitOfWork` wraps `IUserRepository` and exposes `CommitAsync()`. All write paths in `AccountService` go through `_unitOfWork.CommitAsync()` rather than calling `SaveChangesAsync()` directly.

- **Token blacklist** (`BlackListService` in Infrastructure): In-memory `ConcurrentBag<string>` keyed by raw JWT string. `BlackListMiddleware` checks this before the auth middleware runs. **Not persistent** — revoked tokens are re-valid after a restart.

- **Validators are injected explicitly** (`AccountService` constructor receives `RegisterRequestValidator`, `LoginRequestValidator`, etc.) rather than being auto-invoked by the pipeline. Registered in `CleanAuth.Application/DependencyInjection.cs` via `AddApplication()`.

- **`Gender` enum** (`CleanAuth.Domain/Enums/Gender.cs`): `Male = 0`, `Female = 1`. Stored as an integer column on `User`. `RegisterRequest` accepts it as an integer; defaults to `Male` if omitted.

- **`DateOfBirth`**: nullable `DateTime?` on `User`, passed through `RegisterRequest.DateOfBirth`. Optional — no validation applied.

- **Email codes**: 6-char uppercase alphanumeric, 5-minute TTL for email confirmation, 15-minute TTL for password reset. Stored directly on the `User` entity — no separate codes table.

- **Refresh tokens**: Stored on `User` as a base64-encoded random string, 7-day TTL. Rotated on every refresh call.

- **`RememberMe`** (`LoginRequest.RememberMe`): optional bool, default `false`. When `true`, `JwtService.GenerateSecurityToken` issues a 30-day access token instead of the default `_config.Expiration` hours. Refresh token TTL is unaffected (always 7 days).

- **JWT config** (`CleanAuth.Infrastructure/Configs/JwtConfig.cs`) is bound from `appsettings.json` section `"JWT"` and registered as a singleton in `AddInfrastructure()`. Uses HMAC-SHA512, audience/issuer validation is disabled.

- **Database auto-migrate on startup**: `DatabaseExtensions.InitialiseDatabaseAsync()` (in `CleanAuth.Infrastructure/Extensions/`) runs `MigrateAsync()` + `SeedAsync()` in Development mode only.

- **Serilog** is configured via `appsettings.json` (`"Serilog"` section). Writes to console and daily rolling files under `Logs/`. `builder.Logging.ClearProviders()` disables the default Microsoft logger before Serilog is wired in.

- **wwwroot/index.html**: Vanilla JS API console served as static files. Uses `Fraunces` + `IBM Plex Sans` fonts, newspaper black + terracotta color scheme. All API endpoints are exercisable from here without Swagger.

## Configuration

Required `appsettings.json` sections (in `CleanAuth.API/`):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=AuthDb;Username=postgres;Password=root"
  },
  "JWT": {
    "Secret": "<at least 64 bytes>",
    "Issuer": "https://localhost:7228",
    "Audience": "https://localhost:7228",
    "Expiration": 120
  },
  "EmailConfig": {
    "From": "...",
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "UserName": "...",
    "Password": "<Gmail App Password>"
  },
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File"],
    "MinimumLevel": { "Default": "Information", "Override": { "Microsoft": "Warning" } },
    "WriteTo": [{ "Name": "Console" }, { "Name": "File", "Args": { "path": "Logs/log-.txt", "rollingInterval": "Day" } }]
  }
}
```

Docker database defaults: `postgres/postgres`, database `AuthDb`, port 5432.

## Project Structure

```
CleanAuth.slnx                          # Solution file
compose.yaml                            # Docker Compose (PostgreSQL)

CleanAuth.Domain/                       # No external dependencies
├── Common/
│   └── ResponseModel.cs               # ResponseModel + ResponseModel<T>
├── Entities/
│   ├── BaseEntity.cs                  # Guid Id
│   └── User.cs
├── Enums/
│   └── Gender.cs
├── Exceptions/
│   └── ApiException.cs
└── GlobalUsings.cs

CleanAuth.Application/                  # Depends on Domain only (no ASP.NET Core framework ref)
├── DependencyInjection.cs             # AddApplication() extension method
├── DTOs/                              # Request/response DTOs (11 files)
├── Helpers/
│   ├── PasswordHelper.cs              # BCrypt hash/verify
│   └── CodeHelper.cs                  # Random alphanumeric code generation
├── Repositories/
│   ├── IUnitOfWork.cs
│   └── IUserRepository.cs
├── Services/
│   ├── IAccountService.cs / AccountService.cs
│   ├── IEmailService.cs
│   ├── IJwtService.cs
│   ├── ICurrentUserService.cs
│   └── IBlackListService.cs
├── Validators/                        # FluentValidation validators + extensions
└── GlobalUsings.cs

CleanAuth.Infrastructure/               # Depends on Domain + Application
├── DependencyInjection.cs             # AddInfrastructure(IConfiguration) extension method
├── Configs/
│   ├── JwtConfig.cs
│   └── EmailConfig.cs
├── Data/
│   └── AuthContext.cs
├── Extensions/
│   └── DatabaseExtensions.cs         # InitialiseDatabaseAsync + SeedAsync (startup only)
├── Migrations/
├── Repositories/
│   ├── UnitOfWork.cs
│   └── UserRepository.cs
├── Services/
│   ├── JwtService.cs
│   ├── EmailService.cs
│   ├── BlackListService.cs
│   └── CurrentUserService.cs
└── GlobalUsings.cs

CleanAuth.API/                          # Depends on Application + Infrastructure
├── Controllers/
│   ├── AccountController.cs           # All auth endpoints
│   └── OldDaysController.cs           # Demo protected endpoint
├── Middlewares/
│   ├── ExceptionMiddleware.cs
│   └── BlackListMiddleware.cs
├── wwwroot/
│   └── index.html                    # Vanilla JS API console
├── Program.cs                         # Lean: AddApplication() + AddInfrastructure() + Serilog
├── GlobalUsings.cs
└── appsettings.json
```

## Conventions

- Controllers return `ActionResult<ResponseModel<T>>` and delegate all logic to `IAccountService`.
- Services return `ResponseModel` / `ResponseModel<T>` and catch all exceptions internally, logging via `ILogger`.
- All DB writes go through `IUnitOfWork.CommitAsync()`.
- Entities inherit from `BaseEntity` (provides `Id` as `Guid`).
- Static utility logic (hashing, code generation) lives in `CleanAuth.Application/Helpers/` as static classes.
- Swagger XML comments are generated in Debug builds and included in the Swagger UI — keep XML doc comments on all public controller actions and DTOs.
- Each project has its own `GlobalUsings.cs`; add new global usings to the relevant project's file.
- Namespaces follow `CleanAuth.<Project>.<Folder>` (e.g. `CleanAuth.Domain.Entities`, `CleanAuth.Application.Services`, `CleanAuth.Infrastructure.Data`).
- DI registration lives in `DependencyInjection.cs` at the root of each layer — never register Application or Infrastructure services directly in `Program.cs`.
