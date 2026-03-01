namespace Backend_Amethyst_Audio.DTO;

public class ChangeAlbumInfoDto
{
    public IFormFile? CoverFile { get; set; }
    public List<string>? AddedTrackList { get; set; }
}