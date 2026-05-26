namespace CleanAuth.Application.DTOs;

/// <summary>
/// Data transfer object representing user information
/// </summary>
public class UserResponse
{
    /// <summary>
    /// Unique identifier of the user
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Email address of the user
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Username of the user
    /// </summary>
    public required string UserName { get; set; }
}
