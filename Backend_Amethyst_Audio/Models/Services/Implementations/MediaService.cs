using Backend_Amethyst_Audio.Models.Enums;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Backend_Amethyst_Audio.Services.Implementations;

public class MediaService: IMediaSevice
{
    public string RootPath { get; } = "C:\\AmethystAudioMedia\\";

    // Записывает файл в папку, на выходе путь до файла для записи в бд
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
        var path = Path.Combine(RootPath, folderName, fileName);

        Directory.CreateDirectory(Path.GetDirectoryName(path));

        await using FileStream stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        return fileName;
    }
    
    
    public async Task DeleteFileAsync(string filePath)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetTrackFilePathAsync(int id)
    {
        return string.Empty;
    }

    public async Task<string> GetTrackCoverPathAsync(int id)
    {
        return string.Empty;
    }

    public async Task<string> GetPlaylistCoverPathAsync(int id)
    {
        return string.Empty;
    }

    public async Task<string> GetAlbumCoverPathAsync(int id)
    {
        return string.Empty;
    }

    public async Task<string> GetUserAvatarPathAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetUserHeaderPathAsync(int id)
    {
        throw new NotImplementedException();
    }
}