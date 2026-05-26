namespace CleanAuth.Application.Repositories;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    Task<bool> CommitAsync(CancellationToken cancellationToken = default);
}
