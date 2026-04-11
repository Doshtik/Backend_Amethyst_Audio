namespace Backend_Amethyst_Audio.DTO;

public class CreateTrackDto
{
    public string Name { get; set; } = null!;

    public IFormFile CoverFile { get; set; } = null!;

    public IFormFile TrackFile { get; set; } = null!;
    
    public List<UserInfoDto> Authors { get; set; }
    
    public string? PaceName { get; set; }

    public string? MoodName { get; set; }

    public bool? IsTextless { get; set; }

    public bool? IsExplicit { get; set; }

    public string? Country { get; set; }
}