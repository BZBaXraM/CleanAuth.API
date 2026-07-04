namespace CleanAuth.Application.Extensions;

public static class MappingExtension
{
    public static UserResponse ToUserDto(this User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName
        };
    }
}