namespace CleanAuth.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services
            .AddScoped<RegisterRequestValidator>()
            .AddScoped<LoginRequestValidator>()
            .AddScoped<ConfirmEmailCodeDtoValidator>()
            .AddScoped<RequestConfirmationCodeDtoValidator>()
            .AddScoped<ResetPasswordRequestValidator>()
            .AddScoped<ChangePasswordRequestValidator>()
            .AddScoped<ChangeUsernameRequestValidator>()
            .AddValidatorsFromAssemblyContaining<RegisterRequestValidator>()
            .AddScoped<IAccountService, AccountService>();

        return services;
    }
}
