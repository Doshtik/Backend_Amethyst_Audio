using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models.Entities;

namespace Backend_Amethyst_Audio.Services.Abstractions;

public interface IAlbumService
{
    Task<AlbumInfoDto> GetByIdAsync(long id);
    Task<List<AlbumInfoDto>> GetAllAsync();
    
    Task<AlbumInfoDto> CreateAsync(long userId, CreateAlbumDto album);
    Task<AlbumInfoDto> UpdateAsync(ChangeAlbumInfoDto album);
    Task DeleteAsync(long id);

    Task<List<AlbumInfoDto>> GetListOfNewestAsync();
    
    Task<List<AlbumInfoDto>> GetListByAlbumNameAsync(string search);
    Task<List<AlbumInfoDto>> GetListByUserIdAsync(long userId);
    Task<List<AlbumInfoDto>> GetListSavedAsync(long userId);
    
    Task SaveAlbumAsync(long idUser, long idAlbum);
    Task UnsaveAlbumAsync(long idUser, long idAlbum);
}