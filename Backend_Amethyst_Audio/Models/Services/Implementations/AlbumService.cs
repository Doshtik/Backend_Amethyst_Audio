using AutoMapper;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models.Data;
using Backend_Amethyst_Audio.Models.Entities;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Backend_Amethyst_Audio.Services.Implementations;

public class AlbumService : IAlbumService
{
    private readonly AppDbContext _db;
    private readonly ILogger<AlbumService> _logger;
    private readonly IMapper _mapper;
    public AlbumService(AppDbContext db, ILogger<AlbumService> logger, IMapper mapper)
    {
        _db = db;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<AlbumInfoDto> GetByIdAsync(long id)
    {
        Album? album = await _db.Albums.FirstOrDefaultAsync(x => x.Id == id);

        if (album is null)
        {
            _logger.LogError("Album Not Found");
            throw new KeyNotFoundException($"Album Not Found: {id}");
        }
        
        return _mapper.Map<AlbumInfoDto>(album);
    }

    public Task<List<AlbumInfoDto>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<AlbumInfoDto> CreateAsync(CreateAlbumDto album)
    {
        throw new NotImplementedException();
    }

    public async Task<AlbumInfoDto> UpdateAsync(ChangeAlbumInfoDto album)
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