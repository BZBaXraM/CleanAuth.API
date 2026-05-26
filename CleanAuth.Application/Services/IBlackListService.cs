namespace CleanAuth.Application.Services;

public interface IBlackListService
{
    /// <summary>
    /// Check if a token is blacklisted
    /// </summary>
    bool IsTokenBlackListed(string token);

    /// <summary>
    /// Add a token to the black list
    /// </summary>
    void AddTokenToBlackList(string token);
}
