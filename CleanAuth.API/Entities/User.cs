namespace CleanAuth.API.Entities;

public class User
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public DateTime DateOfBirth { get; set; }
    public required Gender Gender { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? EmailConfirmationCodeExpireTime { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public string? EmailConfirmationCode { get; set; }
    public DateTime RefreshTokenExpireTime { get; set; }
}