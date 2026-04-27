using Backend_Amethyst_Audio.Models.Enums;

namespace Backend_Amethyst_Audio.Services.Abstractions;

public interface IMediaService
{
    Task<string> SaveFileAsync(IFormFile file, FileTypes typeName);
    Task DeleteFileAsync(string fileName, FileTypes typeName);
    
    Task<string> GetTrackFilePathAsync(int id);
    Task<string> GetTrackCoverPathAsync(int id);
    Task<string> GetPlaylistCoverPathAsync(int id);
    Task<string> GetAlbumCoverPathAsync(int id);
    Task<string> GetUserAvatarPathAsync(int id);
    Task<string> GetUserHeaderPathAsync(int id);
}