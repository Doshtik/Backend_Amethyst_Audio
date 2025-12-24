namespace Backend_Amethyst_Audio.Services.Abstractions;

public interface IMediaSevice
{
    Task<string> SaveFileAsync(IFormFile file);
    Task DeleteFileAsync(string fileName);
}