namespace Backend_Amethyst_Audio.DTO;

public class ExternalLoginDto
{
    public string Provider { get; set; } = null!;
    public string Token { get; set; } = null!;
}