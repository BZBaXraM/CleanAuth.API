namespace CleanAuth.Application.Services;

public interface ICurrentUserService
{
    Guid? UserId { get; }
}