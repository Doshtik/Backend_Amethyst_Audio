namespace Backend_Amethyst_Audio.Services.Abstractions;

public interface IAuthService
{
    Task<string> HashPassword(string password);
    Task<bool> VerifyPassword(string hashedPassword, string providedPassword);
}