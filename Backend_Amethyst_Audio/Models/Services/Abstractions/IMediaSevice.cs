using Backend_Amethyst_Audio.Models.Enums;

namespace Backend_Amethyst_Audio.Services.Abstractions;

public interface IMediaSevice
{
    Task<string> SaveFileAsync(IFormFile file, FileTypes typeName);
    Task DeleteFileAsync(string filePath);
}