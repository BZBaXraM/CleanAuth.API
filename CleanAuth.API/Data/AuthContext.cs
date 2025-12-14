namespace CleanAuth.API.Data;

public class AuthContext : DbContext
{
    public AuthContext(DbContextOptions<AuthContext> options) : base(options)
    {
    }

    public virtual DbSet<User> Users => Set<User>();
}