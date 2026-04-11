namespace Backend_Amethyst_Audio.DTO;

public class ChangeTrackInfoDto
{
    public int Id { get; set; }
    
    public string? Name { get; set; }

    public IFormFile? CoverFile { get; set; }

    public IFormFile? TrackFile { get; set; }
    
    public string? PaceName { get; set; }

    public string? MoodName { get; set; }

    public bool? IsTextless { get; set; }

    public bool? IsExplicit { get; set; }

    public string? Country { get; set; }
}