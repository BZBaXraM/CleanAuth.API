# 🔐 CleanAuth Microservices Solution

> Complete authentication system and microservices on .NET 10 with JWT authorization

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=flat-square&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![JWT](https://img.shields.io/badge/JWT-000000?style=flat-square&logo=jsonwebtokens&logoColor=white)](https://jwt.io/)
[![BCrypt](https://img.shields.io/badge/BCrypt-4A90E2?style=flat-square)](https://github.com/BcryptNet/bcrypt.net)
[![Docker](https://img.shields.io/badge/Docker-2496ED?style=flat-square&logo=docker&logoColor=white)](https://www.docker.com/)

## 🏗️ Solution Architecture

This solution consists of three interconnected projects:

### 1. 🔐 CleanAuth.API - Authentication Service

Main authentication service with full implementation without ASP.NET Core Identity

### 2. 📦 JwtAuthForMicroservice - JWT Library

Reusable library for JWT authentication in microservices

### 3. 🚀 MiniMicroservice.API - Example Microservice

Demonstration microservice using JWT library for authorization

## 🚀 Key Features

### CleanAuth.API

- **🎯 Without ASP.NET Core Identity** - fully custom implementation
- **🏗️ Clean Architecture** - clear separation of layers and responsibilities
- **📧 Email confirmation** - automatic sending of confirmation codes
- **🔒 JWT Authentication** - with refresh tokens and blacklist
- **✅ Result Pattern** - elegant error handling without exceptions
- **🛡️ FluentValidation** - strict validation of all input data
- **� Structured Logging** - detailed logging of all operations
- **� BCTrypt** - reliable password hashing

### JwtAuthForMicroservice

- **� Reрusable Library** - easy integration into any microservice
- **�️ JWT tMiddleware** - automatic token processing
- **⚡ Easy Setup** - simple registration via extension methods
- **🔍 Token Validation** - signature verification and claims extraction

### MiniMicroservice.API

- **� ProteMcted Endpoints** - all APIs require authorization
- **👥 User Management** - demonstration functionality
- **� Ready to Use** - example of JWT library integration

## 🏛️ System Architecture

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           CleanAuth Microservices Solution                      │
└─────────────────────────────────────────────────────────────────────────────────┘
                                        │
                ┌───────────────────────┼───────────────────────┐
                │                       │                       │
                ▼                       ▼                       ▼
┌─────────────────────┐    ┌─────────────────────┐    ┌─────────────────────┐
│   CleanAuth.API     │    │JwtAuthForMicroservice│    │ MiniMicroservice.API│
│                     │    │                     │    │                     │
│ ┌─────────────────┐ │    │ ┌─────────────────┐ │    │ ┌─────────────────┐ │
│ │   Controllers   │ │    │ │  JwtRegister    │ │    │ │   Controllers   │ │
│ │ • AccountCtrl   │ │    │ │ • Extension     │ │    │ │ • PersonsCtrl   │ │
│ │ • OldDaysCtrl   │ │    │ │   Methods       │ │    │ │ • Protected     │ │
│ └─────────────────┘ │    │ └─────────────────┘ │    │ └─────────────────┘ │
│ ┌─────────────────┐ │    │ ┌─────────────────┐ │    │                     │
│ │    Services     │ │    │ │   Middlewares   │ │    │ Uses JWT Library    │
│ │ • AccountSvc    │ │    │ │ • JwtMiddleware │ │    │ for Authentication  │
│ │ • EmailSvc      │ │    │ │ • Token Valid   │ │    │                     │
│ │ • JwtSvc        │ │    │ └─────────────────┘ │    │                     │
│ └─────────────────┘ │    │ ┌─────────────────┐ │    │                     │
│ ┌─────────────────┐ │    │ │    Services     │ │    │                     │
│ │  Repositories   │ │    │ │ • AccessToken   │ │    │                     │
│ │ • UserRepo      │ │    │ │   Service       │ │    │                     │
│ │ • EF Core       │ │    │ └─────────────────┘ │    │                     │
│ └─────────────────┘ │    └─────────────────────┘    └─────────────────────┘
└─────────────────────┘
         │
         ▼
┌─────────────────────┐
│    PostgreSQL       │
│                     │
│ • AuthDb Database   │
│ • Docker Container  │
│ • User Data         │
└─────────────────────┘
```

### Key Principles

- **Microservices Architecture** - independent, loosely coupled services
- **Single Responsibility** - each class has one responsibility
- **Dependency Injection** - loose coupling of components
- **Result Pattern** - explicit error handling without exceptions
- **Repository Pattern** - data access abstraction
- **Service Layer** - all business logic in services
- **JWT Authorization** - unified authentication system for all microservices

## 🛠️ Technology Stack

### CleanAuth.API

| Component            | Technology                      | Version |
| -------------------- | ------------------------------- | ------- |
| **Runtime**          | .NET                            | 10.0    |
| **Database**         | PostgreSQL                      | Latest  |
| **ORM**              | Entity Framework Core           | 10.0.1  |
| **Authentication**   | JWT Bearer                      | 10.0.1  |
| **Password Hashing** | BCrypt.Net-Next                 | 4.0.3   |
| **Validation**       | FluentValidation                | 12.1.1  |
| **Email**            | MailKit                         | 4.14.1  |
| **Tokens**           | System.IdentityModel.Tokens.Jwt | 8.15.0  |

### JwtAuthForMicroservice

| Component          | Technology                      | Version |
| ------------------ | ------------------------------- | ------- |
| **Runtime**        | .NET                            | 10.0    |
| **Authentication** | JWT Bearer                      | 10.0.1  |
| **Tokens**         | System.IdentityModel.Tokens.Jwt | 8.15.0  |

### MiniMicroservice.API

| Component       | Technology                   | Version         |
| --------------- | ---------------------------- | --------------- |
| **Runtime**     | .NET                         | 10.0            |
| **OpenAPI**     | Microsoft.AspNetCore.OpenApi | 10.0.0          |
| **JWT Library** | JwtAuthForMicroservice       | Local Reference |

## 🚀 Quick Start

### Prerequisites

- .NET 10 SDK
- Docker & Docker Compose (for database)
- SMTP server (Gmail/Outlook)

### Installation and Setup

1. **Clone the repository**

```bash
git clone https://github.com/BZBaXraM/CleanAuth.API.git
cd CleanAuth.API
```

2. **Start PostgreSQL with Docker Compose**

```bash
# Start the database container
docker compose up -d

# The database will be available at:
# Host: localhost
# Port: 5432
# Database: AuthDb
# Username: postgres
# Password: toor
```

3. **Setup database**

```bash
# Navigate to the main project folder
cd CleanAuth.API

# Run migrations to create tables
dotnet ef database update
```

4. **Configure Email (for CleanAuth.API)**

Edit `appsettings.json`:

```json
{
  "EmailConfig": {
    "From": "your-email@gmail.com",
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "UserName": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

5. **Run the services**

```bash
# Start the main authentication service
cd CleanAuth.API
dotnet run

# In a new terminal - start the microservice
cd MiniMicroservice.API
dotnet run
```

### Default Ports

- **CleanAuth.API**: `https://localhost:7228`
- **MiniMicroservice.API**: `https://localhost:7030`
- **PostgreSQL**: `localhost:5432`

### Docker Commands

```bash
# Start database
docker compose up -d

# Stop database
docker compose down

# View logs
docker compose logs auth-db

# Remove database with data
docker compose down -v
```

## 📡 API Endpoints

### 🔐 CleanAuth.API - Authentication Service

#### User Registration

```http
POST /api/account/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123",
  "username": "username",
  "dateOfBirth": "1990-01-01T00:00:00Z",
  "gender": 0
}
```

**Response:**

```json
{
  "success": true,
  "message": "Registration successful. Confirmation code sent to your email.",
  "email": "user@example.com"
}
```

#### Email Confirmation

```http
POST /api/account/confirm-email-code
Content-Type: application/json

{
  "code": "ABC123"
}
```

#### User Login

```http
POST /api/account/login
Content-Type: application/json

{
  "usernameOrEmail": "user@example.com",
  "password": "Password123"
}
```

**Response:**

```json
{
  "token": "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "R3LZBc8fuP/Q4Ge9YdHgqD61XyicXhuReQVPZTP61q0=",
  "refreshTokenExpireTime": "2025-01-20T10:30:00Z"
}
```

#### Token Refresh

```http
POST /api/account/refresh-token
Content-Type: application/json

{
  "refreshToken": "your-refresh-token"
}
```

#### Logout

```http
POST /api/account/logout
Authorization: Bearer your-jwt-token
Content-Type: application/json

{
  "token": "your-jwt-token"
}
```

#### Get Current User

```http
GET /api/account/me
Authorization: Bearer your-jwt-token
```

#### Nostalgic Endpoint

```http
GET /api/olddays/back-to-2022
Authorization: Bearer your-jwt-token
```

**Response:**

```json
"Теплом так веет от старых комнат..."
```

### 🚀 MiniMicroservice.API - Demo Microservice

#### Get Users List

```http
GET /api/persons/get-all
Authorization: Bearer your-jwt-token
```

**Response:**

```json
["Bahram Bayramzade", "Nadir Zamanov", "Gulya Abbasova", "Kenan Aliyev"]
```

> **Note:** All microservice endpoints require JWT token obtained from CleanAuth.API

## 🔒 Security

### Password Validation

- Minimum 8 characters, maximum 30
- Required: uppercase letter, lowercase letter, digit
- Hashing with BCrypt

### User Validation

- Username: 3-20 characters, only letters, digits, underscores
- Email: standard email validation
- Age: minimum 13 years

### JWT Tokens

- Signed with secret key
- Refresh tokens with 7-day expiration
- Blacklist for revoked tokens

### Email Confirmation

- 6-character codes (letters + digits)
- 5-minute expiration
- Automatic sending on registration

## 🎯 Result Pattern

The project uses Result Pattern for elegant error handling:

```csharp
// Base Result
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }
}

// Specialized Result classes
public class AuthResult : Result
{
    public User? User { get; }
    public string? Message { get; }
}
```

**Benefits:**

- ✅ Explicit error handling
- ✅ Type safety
- ✅ Better code readability
- ✅ No exceptions for business logic

## 📁 Project Structure

### CleanAuth.API - Main Authentication Service

```
CleanAuth.API/
├── 📁 Controllers/          # API controllers
│   ├── AccountController.cs # Authentication controller
│   └── OldDaysController.cs # Nostalgic endpoint
├── 📁 Services/             # Business logic
│   ├── AccountService.cs    # Main authentication service
│   ├── EmailService.cs      # Email sending service
│   └── JwtService.cs        # JWT service
├── 📁 Repositories/         # Data access
│   └── UserRepository.cs    # User repository
├── 📁 Common/               # Result Pattern
│   ├── Result.cs           # Base Result
│   ├── AuthResult.cs       # Result for authentication
│   └── EmailResult.cs      # Result for email operations
├── 📁 DTOs/                # Data Transfer Objects
├── 📁 Validators/          # FluentValidation validators
├── 📁 Entities/            # Data models
├── 📁 Configs/             # Configurations
├── 📁 Middlewares/         # Middleware components
├── 📁 Exceptions/          # Custom exceptions
├── 📁 Data/                # Database context
├── 📁 Migrations/          # EF Core migrations
└── 📁 Extensions/          # Extension methods
```

### JwtAuthForMicroservice - JWT Library

```
JwtAuthForMicroservice/
├── 📁 Configs/             # Configurations
│   └── JwtConfig.cs        # JWT configuration
├── 📁 Services/            # Services
│   ├── IAccessTokenService.cs    # Token service interface
│   └── AccessTokenService.cs     # Token service implementation
├── 📁 Middlewares/         # Middleware
│   └── JwtMiddleware.cs    # JWT middleware for validation
└── JwtRegister.cs          # Extension methods for registration
```

### MiniMicroservice.API - Demo Microservice

```
MiniMicroservice.API/
├── 📁 Controllers/         # API controllers
│   └── PersonsController.cs # Users controller
├── 📁 Properties/          # Project properties
├── Program.cs              # Application entry point
├── appsettings.json        # Application configuration
└── MiniMicroservice.API.http # HTTP requests for testing
```

## � Connfiguration

### CleanAuth.API - appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=AuthDb;Username=postgres;Password=toor",
    "DockerConnection": "Host=localhost;Database=AuthDb;Username=postgres;Password=toor"
  },
  "JWT": {
    "Secret": "your-super-secret-jwt-key-here-must-be-long-enough"
  },
  "EmailConfig": {
    "From": "your-email@gmail.com",
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "UserName": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

### MiniMicroservice.API - appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "JWT": {
    "Secret": "your-super-secret-jwt-key-here-must-be-long-enough"
  },
  "AllowedHosts": "*"
}
```

> **Important:** JWT Secret must be the same across all services for proper authentication

### Gmail App Password Setup

1. Enable 2FA in your Google account
2. Create App Password in security settings
3. Use this password in configuration

### Environment Variables

```bash
export ConnectionStrings__DefaultConnection="Host=localhost;Database=AuthDb;Username=postgres;Password=toor"
export JWT__Secret="your-jwt-secret"
export EmailConfig__Password="your-email-password"
```

## 🐳 Docker Setup

The project includes a `compose.yaml` file for easy PostgreSQL setup:

```yaml
services:
  auth--db:
    container_name: auth-db
    image: postgres:latest
    environment:
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=toor
      - POSTGRES_DB=AuthDb
    restart: always
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

volumes:
  postgres_data:
```

**Database Connection Details:**

- **Host:** localhost
- **Port:** 5432
- **Database:** AuthDb
- **Username:** postgres
- **Password:** toor

## 🔄 Microservices Integration

### How to use JWT library in a new microservice

1. **Add project reference**

```xml
<ProjectReference Include="..\JwtAuthForMicroservice\JwtAuthForMicroservice.csproj" />
```

2. **Register JWT in Program.cs**

```csharp
using JwtAuthForMicroservice;
using JwtAuthForMicroservice.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Register JWT authentication
builder.Services.RegisterJwt(builder.Configuration);

var app = builder.Build();

// Add JWT middleware
app.UseMiddleware<JwtMiddleware>();
app.UseAuthorization();
```

3. **Protect controllers**

```csharp
[ApiController, Authorize]
[Route("api/[controller]")]
public class YourController : ControllerBase
{
    // Your protected endpoints
}
```

4. **Configure JWT settings**

Ensure JWT Secret in `appsettings.json` matches CleanAuth.API

## 🔍 System Testing

### Complete Testing Scenario

1. **Start the database**

```bash
docker compose up -d
```

2. **Start CleanAuth.API**

```bash
cd CleanAuth.API
dotnet run
```

3. **Register a user**

```bash
curl -X POST https://localhost:7228/api/account/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Password123",
    "username": "testuser",
    "dateOfBirth": "1990-01-01T00:00:00Z",
    "gender": 0
  }'
```

4. **Confirm email** (check code in email)

```bash
curl -X POST https://localhost:7228/api/account/confirm-email-code \
  -H "Content-Type: application/json" \
  -d '{"code": "YOUR_CODE"}'
```

5. **Login**

```bash
curl -X POST https://localhost:7228/api/account/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "test@example.com",
    "password": "Password123"
  }'
```

6. **Start microservice**

```bash
cd MiniMicroservice.API
dotnet run
```

7. **Test protected microservice endpoint**

```bash
curl -X GET https://localhost:7030/api/persons/get-all \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## 🚀 Production Deployment

### Production Recommendations

1. **Use HTTPS everywhere**
2. **Configure secure JWT secrets**
3. **Use external PostgreSQL database**
4. **Setup logging (Serilog, NLog)**
5. **Add monitoring (Prometheus, Grafana)**
6. **Use API Gateway for routing**
7. **Setup CI/CD pipeline**

### Docker Compose for Production

```yaml
version: "3.8"
services:
  cleanauth-api:
    build: ./CleanAuth.API
    ports:
      - "5001:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=AuthDb;Username=postgres;Password=your-secure-password
    depends_on:
      - postgres

  mini-microservice:
    build: ./MiniMicroservice.API
    ports:
      - "5002:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    depends_on:
      - cleanauth-api

  postgres:
    image: postgres:15
    environment:
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=your-secure-password
      - POSTGRES_DB=AuthDb
    volumes:
      - postgres_data:/var/lib/postgresql/data
    ports:
      - "5432:5432"

volumes:
  postgres_data:
```

## 👨‍💻 Author

**Bahram Bayramzade**

- GitHub: [@BZBaXraM](https://github.com/BZBaXraM)
- Email: baxram1997007@gmail.com

## 🙏 Acknowledgments

- [BCrypt.Net](https://github.com/BcryptNet/bcrypt.net) - for reliable password hashing
- [FluentValidation](https://fluentvalidation.net/) - for elegant validation
- [MailKit](https://github.com/jstedfast/MailKit) - for email functionality
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) - for database operations
- [JWT](https://jwt.io/) - for authentication tokens

---

⭐ **Star this project if it was helpful!**
