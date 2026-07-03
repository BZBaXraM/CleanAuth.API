namespace CleanAuth.Application.DTOs;

/// <summary>
/// Request model for changing the username of the currently authenticated user
/// </summary>
public class ChangeUsernameRequest
{
    /// <summary>
    /// New username (3-20 characters, letters, numbers, and underscores only)
    /// </summary>
    public string NewUsername { get; set; } = null!;
}
