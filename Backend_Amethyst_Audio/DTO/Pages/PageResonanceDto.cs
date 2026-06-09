namespace Backend_Amethyst_Audio.DTO.Pages;

public class PageResonanceDto
{
    public short PaceId { get; set; }
    public short MoodId { get; set; }
    public bool IsFromLibrary { get; set; }
    public bool IsTextless { get; set; }
    public string? Country { get; set; }
}