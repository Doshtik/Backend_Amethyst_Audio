using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Services.Abstractions;

namespace Backend_Amethyst_Audio.Services.Implementations;

public class AlbumService : IAlbumService
{
    public Task<AlbumInfoDto> GetByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<List<AlbumInfoDto>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task CreateAsync(CreateAlbumDto album)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(ChangeAlbumInfoDto album)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<List<AlbumInfoDto>> GetListOfNewestAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<AlbumInfoDto>> GetListBySearchAsync(string search)
    {
        throw new NotImplementedException();
    }

    public Task<List<AlbumInfoDto>> GetListByUserIdAsync(long userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<AlbumInfoDto>> GetListSavedAsync(long userId)
    {
        throw new NotImplementedException();
    }

    public Task SaveAlbumAsync(long idUser, long idAlbum)
    {
        throw new NotImplementedException();
    }

    public Task UnsaveAlbumAsync(long idUser, long idAlbum)
    {
        throw new NotImplementedException();
    }
}