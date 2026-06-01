# CleanAuth.API

Complete authentication service built with .NET 10 and PostgreSQL, structured as a Clean Architecture solution. No ASP.NET Core Identity — everything is custom-built.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-316192?style=flat-square&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![JWT](https://img.shields.io/badge/JWT-HMAC--SHA512-000000?style=flat-square&logo=jsonwebtokens&logoColor=white)](https://jwt.io/)
[![Docker](https://img.shields.io/badge/Docker-2496ED?style=flat-square&logo=docker&logoColor=white)](https://www.docker.com/)

## Features

- Custom user management — no ASP.NET Core Identity dependency
- JWT authentication with refresh token rotation, in-memory blacklist, and "Remember me" (30-day token)
- Email verification with 6-char codes (5-minute TTL)
- Password reset flow with 6-char codes (15-minute TTL)
- `ResponseModel<T>` pattern — services never throw for business logic errors
- Unit of Work wrapping all write paths through a single `CommitAsync()`
- FluentValidation on all inputs, injected explicitly into services
- BCrypt password hashing (cost factor 12)
- Serilog structured logging — console + daily rolling files under `Logs/`
- Swagger/OpenAPI with XML doc comments
- Vanilla JS API console at `/` (no Swagger required for testing)
- Docker Compose for local PostgreSQL
- Clean Architecture with 4 separate projects and per-layer `DependencyInjection.cs`

## Quick Start

**Prerequisites:** .NET 10 SDK, Docker, Gmail App Password (for email)

```bash
# 1. Start the database
docker compose up -d

# 2. Run the API
dotnet run --project CleanAuth.API/CleanAuth.API.csproj
```

API: `https://localhost:7228`  
Swagger: `https://localhost:7228/swagger`  
API Console: `https://localhost:7228`

In Development, EF Core migrations run automatically on startup.

### Email setup (Gmail)

Enable 2FA on your Google account, generate an App Password, then set in `CleanAuth.API/appsettings.json`:

```json
"EmailConfig": {
  "From": "you@gmail.com",
  "SmtpServer": "smtp.gmail.com",
  "Port": 587,
  "UserName": "you@gmail.com",
  "Password": "<app-password>"
}
```

## Configuration

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=AuthDb;Username=postgres;Password=postgres"
  },
  "JWT": {
    "Secret": "<64+ byte secret>",
    "Issuer": "https://localhost:7228",
    "Audience": "https://localhost:7228",
    "Expiration": 120
  },
  "EmailConfig": {
    "From": "you@gmail.com",
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "UserName": "you@gmail.com",
    "Password": "<app-password>"
  },
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File"],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information",
        "Microsoft.EntityFrameworkCore": "Information",
        "System": "Warning"
      }
    },
    "Enrich": ["FromLogContext"],
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "Logs/log-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 14,
          "shared": true
        }
      }
    ]
  }
}
```

Override any value with environment variables using `__` as separator:

```bash
export JWT__Secret="..."
export EmailConfig__Password="..."
```

## API Endpoints

### Identity

| Method | Endpoint | Auth |
|--------|----------|------|
| `POST` | `/api/account/register` | — |
| `POST` | `/api/account/login` | — |
| `GET`  | `/api/account/me` | Bearer |
| `POST` | `/api/account/logout` | Bearer |

### Email Verification

| Method | Endpoint | Auth |
|--------|----------|------|
| `POST` | `/api/account/confirm-email-code` | — |
| `POST` | `/api/account/request-confirmation-code` | — |

### Password

| Method | Endpoint | Auth |
|--------|----------|------|
| `POST` | `/api/account/forget-password` | — |
| `POST` | `/api/account/reset-password` | — |

### Tokens

| Method | Endpoint | Auth |
|--------|----------|------|
| `POST` | `/api/account/refresh-token` | — |

### Register

```http
POST /api/account/register
Content-Type: application/json

{
  "email": "user@example.com",
  "username": "john_doe",
  "password": "Password123!",
  "gender": 0,
  "dateOfBirth": "1990-01-01T00:00:00Z"
}
```

`gender`: `0` = Male, `1` = Female. `dateOfBirth` is optional.

### Login

```http
POST /api/account/login
Content-Type: application/json

{
  "usernameOrEmail": "john_doe",
  "password": "Password123!",
  "rememberMe": false
}
```

`rememberMe`: optional, default `false`. When `true`, the access token is valid for 30 days instead of the default configured expiry (120 min). Refresh token TTL is always 7 days regardless.

```json
{
  "isSucceeded": true,
  "statusCode": 200,
  "data": {
    "token": "eyJ...",
    "refreshToken": "R3LZ...",
    "refreshTokenExpireTime": "2026-05-28T10:30:00Z"
  }
}
```

## Architecture

**Layer dependency direction (strictly inward):**
```
CleanAuth.Domain  (no external deps)
      ↑
CleanAuth.Application  (depends on Domain)
      ↑
CleanAuth.Infrastructure  (depends on Domain + Application)
      ↑
CleanAuth.API  (depends on Application + Infrastructure)
```

**Request flow:**
```
HTTP → ExceptionMiddleware → BlackListMiddleware → Controller → AccountService → UnitOfWork → UserRepository → AuthContext
```

**Layer responsibilities:**

| Layer | Responsibility |
|-------|---------------|
| `CleanAuth.Domain` | Entities (`User`, `BaseEntity`), enums, exceptions, `ResponseModel<T>` |
| `CleanAuth.Application` | Service interfaces, repository interfaces, DTOs, validators, helpers, `AccountService`; registered via `AddApplication()` |
| `CleanAuth.Infrastructure` | EF Core `AuthContext`, repository/service implementations, configs, JWT auth; registered via `AddInfrastructure(config)` |
| `CleanAuth.API` | Controllers, middleware, lean `Program.cs`, Serilog bootstrap, static files |

## Project Structure

```
CleanAuth.slnx
compose.yaml

CleanAuth.Domain/
├── Common/ResponseModel.cs
├── Entities/BaseEntity.cs, User.cs
├── Enums/Gender.cs
└── Exceptions/ApiException.cs

CleanAuth.Application/
├── DependencyInjection.cs     # AddApplication()
├── DTOs/                      # 11 request/response DTOs
├── Helpers/PasswordHelper.cs, CodeHelper.cs
├── Repositories/IUnitOfWork.cs, IUserRepository.cs
├── Services/IAccountService.cs (+ AccountService.cs), IJwtService.cs, IEmailService.cs, IBlackListService.cs, ICurrentUserService.cs
└── Validators/                # FluentValidation validators + extensions

CleanAuth.Infrastructure/
├── DependencyInjection.cs     # AddInfrastructure(IConfiguration)
├── Configs/JwtConfig.cs, EmailConfig.cs
├── Data/AuthContext.cs
├── Extensions/DatabaseExtensions.cs  # InitialiseDatabaseAsync + SeedAsync
├── Migrations/
├── Repositories/UnitOfWork.cs, UserRepository.cs
└── Services/JwtService.cs, EmailService.cs, BlackListService.cs, CurrentUserService.cs

CleanAuth.API/
├── Controllers/AccountController.cs, OldDaysController.cs
├── Middlewares/ExceptionMiddleware.cs, BlackListMiddleware.cs
├── wwwroot/index.html
├── Program.cs                 # Lean: Serilog + AddApplication() + AddInfrastructure()
└── appsettings.json
```

## EF Core Migrations

The `AuthContext` lives in `CleanAuth.Infrastructure`. Always specify both projects when running migration commands:

```bash
dotnet ef migrations add <Name> --project CleanAuth.Infrastructure --startup-project CleanAuth.API
dotnet ef database update --project CleanAuth.Infrastructure --startup-project CleanAuth.API
dotnet ef migrations remove --project CleanAuth.Infrastructure --startup-project CleanAuth.API
```

## Technology Stack

| Component | Library | Version |
|-----------|---------|---------|
| Runtime | .NET | 10.0 |
| Database | PostgreSQL | 17 |
| ORM | Entity Framework Core + Npgsql | 10.0 |
| Auth | Microsoft.AspNetCore.Authentication.JwtBearer | 10.0 |
| Password hashing | BCrypt.Net-Next | 4.0.3 |
| Validation | FluentValidation | 12.1.1 |
| Email | MailKit | 4.14.1 |
| JWT | System.IdentityModel.Tokens.Jwt | 8.x |
| Logging | Serilog.AspNetCore | 10.0 |

## Security Notes

- Passwords: min 8 / max 30 chars, requires uppercase + lowercase + digit
- Usernames: 3–20 chars, letters/digits/underscores only
- JWT: HMAC-SHA512, configurable expiry (default 120 min); 30-day when `rememberMe: true`
- Refresh tokens: 7-day TTL, rotated on every use
- Token blacklist: in-memory only — revoked tokens become valid again on restart
- Auto-migrate runs in Development only

## Docker

```bash
docker compose up -d        # start postgres
docker compose down         # stop
docker compose down -v      # stop + delete data
docker compose logs auth-db # view logs
```

Database: `localhost:5432`, db `AuthDb`, user `postgres`, password `postgres`

## Author

**Bahram Bayramzade** — [@BZBaXraM](https://github.com/BZBaXraM) — baxram1997007@gmail.com