using Backend_Amethyst_Audio.Models;

namespace Backend_Amethyst_Audio.Services.Abstractions;

public interface IAlbumService
{
    Task<Album> GetByIdAsync(long id);
    Task<List<Album>> GetAllAsync();
    
    Task CreateAsync(Album album);
    Task UpdateAsync(Album album);
    Task DeleteAsync(long id);

    Task<List<Album>> GetListOfNewestAsync();
    
    Task<List<Album>> GetListBySearchAsync(string search);
    Task<List<Album>> GetListByUserIdAsync(long userId);
    Task<List<Album>> GetListSavedAsync(long userId);
    
    Task SaveAlbumAsync(long idUser, long idAlbum);
    Task UnsaveAlbumAsync(long idUser, long idAlbum);
}