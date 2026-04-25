namespace Backend_Amethyst_Audio.DTO;

public class ChangeAlbumInfoDto
{
    public string Name { get; set; }
    public IFormFile? CoverFile { get; set; }
    public List<string>? AddedTrackList { get; set; }
}