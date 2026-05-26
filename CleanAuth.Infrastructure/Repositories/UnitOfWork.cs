namespace CleanAuth.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AuthContext _context;
    private IUserRepository? _userRepository;

    public IUserRepository UserRepository =>
        _userRepository ??= new UserRepository(_context);

    public UnitOfWork(AuthContext context) => _context = context;

    public async Task<bool> CommitAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken) > 0;
}
