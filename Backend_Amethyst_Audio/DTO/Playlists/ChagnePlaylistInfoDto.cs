namespace Backend_Amethyst_Audio.DTO;

public class ChagnePlaylistInfoDto
{
    public short? AccessTypeName { get; set; }

    public string? Name { get; set; }

    public string? Discription { get; set; }

    public IFormFile? CoverFile { get; set; }
    
    public List<string>? AddedTrackList { get; set; }
    
    public List<string>? RemovedTrackList { get; set; }
}