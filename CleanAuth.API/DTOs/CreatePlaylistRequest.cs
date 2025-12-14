namespace CleanAuth.API.DTOs;

public class CreatePlaylistRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = true;
    public bool IsCollaborative { get; set; } = false;
}