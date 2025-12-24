namespace Backend_Amethyst_Audio.DTO;

public class UserCreateDTO
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Nickname { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!; // Передаем чистый пароль, который потом хешируем
    public string Gender { get; set; } = null!;
    public string Country { get; set; } = null!;
}