using Backend_Amethyst_Audio.Models.Enums;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Backend_Amethyst_Audio.Services.Implementations;

public class MediaService: IMediaSevice
{
    const string ROOTPATH = "C:\\AmethystAudioMedia\\";

    // Записывает файл в папку, на выходе путь до файла
    public async Task<string> SaveFileAsync(IFormFile file, FileTypes typeName)
    {
        string folderName = typeName switch
        {
            FileTypes.Tracks => folderName = "/Tracks/",
            FileTypes.Covers => folderName = "/Covers/",
            FileTypes.Avatars => folderName = "/Avatars/",
            FileTypes.Headers => folderName = "/Headers/"
        };
        
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var path = Path.Combine(ROOTPATH, folderName, fileName);

        Directory.CreateDirectory(Path.GetDirectoryName(path));

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return fileName;
    }
    
    
    public async Task DeleteFileAsync(string filePath)
    {
        throw new NotImplementedException();
    }
}