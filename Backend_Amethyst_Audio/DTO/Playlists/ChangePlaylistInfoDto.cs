namespace Backend_Amethyst_Audio.DTO;

public class ChangePlaylistInfoDto
{
    public bool? IsPublic { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public IFormFile? CoverFile { get; set; }
    
    public List<TrackInfoDto>? AddedTrackList { get; set; }
    
    public List<TrackInfoDto>? RemovedTrackList { get; set; }
}