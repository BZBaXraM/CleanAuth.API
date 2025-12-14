namespace CleanAuth.API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthContext _authContext;

    public UserRepository(AuthContext authContext)
    {
        _authContext = authContext;
    }

    public async Task<User> AddUserAsync(User user)
    {
        await _authContext.Users.AddAsync(user);
        await _authContext.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _authContext.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _authContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _authContext.Users
            .FirstOrDefaultAsync(u => u.UserName == username);
    }

    public async Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail)
    {
        return await _authContext.Users
            .FirstOrDefaultAsync(u => u.UserName == usernameOrEmail || u.Email == usernameOrEmail);
    }

    public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        return await _authContext.Users
            .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
    }

    public async Task<User?> GetUserByConfirmationCodeAsync(string code)
    {
        return await _authContext.Users
            .FirstOrDefaultAsync(x => x.EmailConfirmationCode == code);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _authContext.SaveChangesAsync() > 0;
    }
}