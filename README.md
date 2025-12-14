# 🔐 CleanAuth API

> Clean authentication system on .NET 10 without ASP.NET Core Identity

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=flat-square&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![JWT](https://img.shields.io/badge/JWT-000000?style=flat-square&logo=jsonwebtokens&logoColor=white)](https://jwt.io/)
[![BCrypt](https://img.shields.io/badge/BCrypt-4A90E2?style=flat-square)](https://github.com/BcryptNet/bcrypt.net)

## 🚀 Features

- **🎯 Without ASP.NET Core Identity** - fully custom implementation
- **🏗️ Clean Architecture** - clear separation of layers and responsibilities
- **📧 Email confirmation** - automatic sending of confirmation codes
- **🔒 JWT Authentication** - with refresh tokens and blacklist
- **✅ Result Pattern** - elegant error handling without exceptions
- **🛡️ FluentValidation** - strict validation of all input data
- **📊 Structured Logging** - detailed logging of all operations
- **🔐 BCrypt** - reliable password hashing

## 🏛️ Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Controllers   │───▶│    Services     │───▶│  Repositories   │
│                 │    │                 │    │                 │
│ • AccountCtrl   │    │ • AccountSvc    │    │ • UserRepo      │
│ • Clean API     │    │ • EmailSvc      │    │ • Data Access   │
│ • Validation    │    │ • JwtSvc        │    │ • EF Core       │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Validators    │    │  Result Pattern │    │   PostgreSQL    │
│                 │    │                 │    │                 │
│ • FluentValid   │    │ • AuthResult    │    │ • Database      │
│ • Business Rules│    │ • EmailResult   │    │ • Migrations    │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

### Key Principles

- **Single Responsibility** - each class has one responsibility
- **Dependency Injection** - loose coupling of components
- **Result Pattern** - explicit error handling without exceptions
- **Repository Pattern** - data access abstraction
- **Service Layer** - all business logic in services

## 🛠️ Technology Stack

| Component            | Technology            | Version |
| -------------------- | --------------------- | ------- |
| **Runtime**          | .NET                  | 10.0    |
| **Database**         | PostgreSQL            | Latest  |
| **ORM**              | Entity Framework Core | 10.0.1  |
| **Authentication**   | JWT Bearer            | 10.0.1  |
| **Password Hashing** | BCrypt.Net-Next       | 4.0.3   |
| **Validation**       | FluentValidation      | 12.1.1  |
| **Email**            | MailKit               | 4.14.1  |

## 🚀 Quick Start

### Prerequisites

- .NET 10 SDK
- PostgreSQL
- SMTP server (Gmail/Outlook)

### Installation

1. **Clone the repository**

```bash
git clone https://github.com/yourusername/CleanAuth.API.git
cd CleanAuth.API
```

2. **Setup database**

```bash
# Update connection string in appsettings.json
dotnet ef database update
```

3. **Configure Email**

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

4. **Run the application**

```bash
dotnet run
```

## 📡 API Endpoints

### 🔐 Authentication

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

### 👤 User

#### Get Current User

```http
GET /api/account/me
Authorization: Bearer your-jwt-token
```

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

```
CleanAuth.API/
├── 📁 Controllers/          # API controllers
│   └── AccountController.cs # Clean controller (only service calls)
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
└── 📁 Exceptions/          # Custom exceptions
```

## 🔧 Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=cleanauth;Username=postgres;Password=password"
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

### Gmail App Password

1. Enable 2FA in your Google account
2. Create App Password in security settings
3. Use this password in configuration

### Environment Variables

```bash
export ConnectionStrings__DefaultConnection="your-db-connection"
export JWT__Secret="your-jwt-secret"
export EmailConfig__Password="your-email-password"
```

## 👨‍💻 Author

**Bahram Bayramzade**

- GitHub: [@BZBaXraM](https://github.com/BZBaXraM)
- Email: baxram1997007@gmail.com

## 🙏 Acknowledgments

- [BCrypt.Net](https://github.com/BcryptNet/bcrypt.net) - for reliable password hashing
- [FluentValidation](https://fluentvalidation.net/) - for elegant validation
- [MailKit](https://github.com/jstedfast/MailKit) - for email functionality

---

⭐ **Star this project if it was helpful!**
