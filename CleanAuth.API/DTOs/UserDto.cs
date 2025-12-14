namespace CleanAuth.API.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string UserName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public required string Gender { get; set; }
}