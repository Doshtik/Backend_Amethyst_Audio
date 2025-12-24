namespace Backend_Amethyst_Audio.DTO;

public class UserReadDTO
{
    public long Id { get; set; }
    public string? Lastname { get; set; }
    public string? Firstname { get; set; }
    public string Nickname { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Gender { get; set; } = null!;
    public string? Country { get; set; }
    // Вместо AvatarName отдаем полную ссылку
    public string? AvatarUrl { get; set; } 
    public string? HeaderUrl { get; set; }
    public bool IsVerified { get; set; }
    public DateTime LastVisit { get; set; }
    public DateTime CreatedAt { get; set; }
}