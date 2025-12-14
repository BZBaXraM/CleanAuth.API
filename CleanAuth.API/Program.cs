var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AuthContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Middlewares
builder.Services.AddSingleton<BlackListMiddleware>();

// Validators
builder.Services
    .AddScoped<RegisterRequestValidator>()
    .AddScoped<LoginRequestValidator>()
    .AddScoped<ConfirmEmailCodeDtoValidator>()
    .AddScoped<RequestConfirmationCodeDtoValidator>();

// Configure Email
EmailConfig emailConfig = new();
builder.Configuration.GetSection("EmailConfig").Bind(emailConfig);
builder.Services.AddSingleton(emailConfig);
builder.Services.AddSingleton<SmtpClient>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services
    .AddScoped<IAccountService, AccountService>()
    .AddScoped<IJwtService, JwtService>()
    .AddSingleton<IEmailService, EmailService>()
    .AddSingleton<IBlackListService, BlackListService>();

// Validators
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

// JWT Authentication
JwtConfig jwtConfig = new();
builder.Configuration.GetSection("JWT").Bind(jwtConfig);
builder.Services.AddSingleton(jwtConfig);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.Secret)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<BlackListMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();