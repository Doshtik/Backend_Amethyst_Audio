using System.ComponentModel.DataAnnotations;

namespace Backend_Amethyst_Audio.DTO;

public class CreateAlbumDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public IFormFile CoverFile { get; set; }
    [Required]
    public List<IFormFile> Tracks { get; set; }
}