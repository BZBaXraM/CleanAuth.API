namespace CleanAuth.API.Extensions;

public static class DatabaseExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var authContext = scope.ServiceProvider.GetRequiredService<AuthContext>();

        await authContext.Database.MigrateAsync();
        await SeedAsync(authContext);
    }

    private static async Task SeedAsync(AuthContext authContext)
    {
        await SeedAuthAsync(authContext);
    }

    private static async Task SeedAuthAsync(AuthContext context)
    {
        if (!await context.Users.AnyAsync())
        {
            await context.Database.ExecuteSqlRawAsync("DELETE FROM  \"Users\"");
            await context.SaveChangesAsync();
        }
    }
}