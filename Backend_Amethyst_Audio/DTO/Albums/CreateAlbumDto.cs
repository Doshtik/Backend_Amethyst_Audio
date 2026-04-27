using System.ComponentModel.DataAnnotations;

namespace Backend_Amethyst_Audio.DTO;

public class CreateAlbumDto
{
    public string Name { get; set; }

    public IFormFile CoverFile { get; set; }
    
    public string AuthorsIdList { get; set; }
    
    public string TracksIdList { get; set; }
}