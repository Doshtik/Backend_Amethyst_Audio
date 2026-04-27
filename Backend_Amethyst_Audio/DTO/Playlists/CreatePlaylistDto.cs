using System.ComponentModel.DataAnnotations;

namespace Backend_Amethyst_Audio.DTO;

public class CreatePlaylistDto
{
    [Required]
    public long UserId { get; set; }

    [Required]
    public bool IsPublic { get; set; }

    [Required]
    public string Name { get; set; }

    public string? Description { get; set; }

    public IFormFile? CoverFile { get; set; }
    
    [Required]
    public string TracksIdList { get; set; }
}