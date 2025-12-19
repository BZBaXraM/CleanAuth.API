using System.Text;
using JwtAuthForMicroservice.Configs;
using JwtAuthForMicroservice.Middlewares;
using JwtAuthForMicroservice.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthForMicroservice;

public static class JwtRegister
{
    public static IServiceCollection RegisterJwt(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAccessTokenService, AccessTokenService>();
        services.AddScoped<JwtMiddleware>();

        JwtConfig jwtConfig = new();
        configuration.GetSection("JWT").Bind(jwtConfig);
        services.AddSingleton(jwtConfig);
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

        return services;
    }
}