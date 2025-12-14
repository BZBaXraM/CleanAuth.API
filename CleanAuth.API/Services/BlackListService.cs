namespace CleanAuth.API.Services;

public class BlackListService : IBlackListService
{
    private ConcurrentBag<string> BlackList { get; set; } = [];

    public bool IsTokenBlackListed(string token)
    {
        return !string.IsNullOrWhiteSpace(token) && BlackList.Contains(token);
    }

    public void AddTokenToBlackList(string token)
    {
        if (!string.IsNullOrWhiteSpace(token) && !BlackList.Contains(token))
        {
            BlackList.Add(token);
        }
    }
}