namespace Backend_Amethyst_Audio.DTO;

public class AlbumInfoDto
{
    public long Id { get; set; }
    
    public string Name { get; set; } = null!;

    public string? CoverUrl { get; set; }
    
    public List<TrackInfoDto> TrackList { get; set; } = null!;
    public List<IFormFile> Tracks { get; set; }
}