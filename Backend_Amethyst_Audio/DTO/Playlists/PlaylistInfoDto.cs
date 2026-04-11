namespace Backend_Amethyst_Audio.DTO;

public class PlaylistInfoDto
{
    public long Id { get; set; }
    
    public long OwnerId { get; set; }
    
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? CoverUrl { get; set; }
    
    public List<TrackInfoDto> TrackList { get; set; } = null!;
}