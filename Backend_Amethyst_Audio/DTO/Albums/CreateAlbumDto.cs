using System.ComponentModel.DataAnnotations;

namespace Backend_Amethyst_Audio.DTO;

public class CreateAlbumDto
{
    public string Name { get; set; }

    public IFormFile CoverFile { get; set; }
    
    public string AuthorIdListJson { get; set; }
    
    public string TrackIdListJson { get; set; }
}