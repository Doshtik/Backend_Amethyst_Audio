using System.ComponentModel.DataAnnotations;

namespace Backend_Amethyst_Audio.DTO;

public class CreatePlaylistDto
{
    [Required]
    public long UserId { get; set; }

    [Required]
    public short AccessTypeName { get; set; }

    [Required]
    public string Name { get; set; }

    public string? Discription { get; set; }

    public IFormFile? CoverFile { get; set; }
}