namespace Backend_Amethyst_Audio.DTO;

public class ChangeAlbumInfoDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public IFormFile? CoverFile { get; set; }
    public string? AddedTrackList { get; set; }
    public string? RemovedTrackList { get; set; }
}