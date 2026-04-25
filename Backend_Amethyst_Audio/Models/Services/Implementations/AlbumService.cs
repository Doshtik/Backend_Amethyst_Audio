using AutoMapper;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models.Data;
using Backend_Amethyst_Audio.Models.Entities;
using Backend_Amethyst_Audio.Models.Enums;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Backend_Amethyst_Audio.Services.Implementations;

public class AlbumService : IAlbumService
{
    private readonly AppDbContext _db;
    private readonly IMediaService _mediaService;
    private readonly ILogger<AlbumService> _logger;
    private readonly IMapper _mapper;

    public AlbumService(AppDbContext db, IMediaService mediaService, ILogger<AlbumService> logger, IMapper mapper)
    {
        _db = db;
        _mediaService = mediaService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<AlbumInfoDto> GetByIdAsync(long id)
    {
        _logger.LogDebug("[Debug] Fetching album with ID: {AlbumId}", id);
        var album = await _db.Albums.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        if (album is null)
        {
            _logger.LogWarning("[Warn] Album with ID {AlbumId} was not found", id);
            throw new KeyNotFoundException($"Album with ID {id} was not found.");
        }

        _logger.LogDebug("[Debug] Successfully fetched album {AlbumId}", id);
        return _mapper.Map<AlbumInfoDto>(album);
    }

    public async Task<List<AlbumInfoDto>> GetAllAsync()
    {
        _logger.LogDebug("[Debug] Fetching all albums");
        var albums = await _db.Albums.AsNoTracking().ToListAsync();
        _logger.LogInformation("[Info] Retrieved {Count} albums", albums.Count);
        return _mapper.Map<List<AlbumInfoDto>>(albums);
    }

    public async Task<AlbumInfoDto> CreateAsync(long userId, CreateAlbumDto dto)
    {
        _logger.LogDebug("[Debug] Creating new album from DTO: {Dto}", dto);
        var albumEntity = _mapper.Map<Album>(dto);
        
        string coverFileName;
        if (dto.CoverFile != null)
        {
            try
            {
                _logger.LogDebug("[Debug] Saving playlist cover file");
                coverFileName = await _mediaService.SaveFileAsync(dto.CoverFile, FileTypes.Covers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Error] Failed to save media files for playlist '{Title}'", dto.Name);
                throw new InvalidOperationException("Failed to upload media playlist. Database record was not created.", ex);
            }
        }
        else
        {
            coverFileName = "default_cover.jpg";
            _logger.LogDebug("[Debug] No cover file provided, using default: {FileName}", coverFileName);
        }
        
        albumEntity.CoverFileName = coverFileName;
        albumEntity.CreatedAt = DateTime.UtcNow;
        albumEntity.UpdatedAt = DateTime.UtcNow;
        
        await _db.Albums.AddAsync(albumEntity);
        await _db.SaveChangesAsync();
        
        await _db.AlbumsAuthors.AddAsync(new AlbumsAuthor { IdAlbum = albumEntity.Id, IdAuthor = userId });
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("[Info] Successfully created album with ID {AlbumId}", albumEntity.Id);
        return _mapper.Map<AlbumInfoDto>(albumEntity);
    }

    public async Task<AlbumInfoDto> UpdateAsync(ChangeAlbumInfoDto dto)
    {
        _logger.LogDebug("[Debug] Updating album with DTO: {@Dto}", dto);
        var albumEntity = _mapper.Map<Album>(dto);
        
        albumEntity.UpdatedAt = DateTime.Now;
        
        _db.Albums.Update(albumEntity);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("[Info] Successfully updated album {AlbumId}", albumEntity.Id);
        return _mapper.Map<AlbumInfoDto>(albumEntity);
    }

    public async Task DeleteAsync(long id)
    {
        _logger.LogDebug("[Debug] Attempting to delete album {AlbumId}", id);
        var albumEntity = await _db.Albums.FindAsync(id);

        if (albumEntity is null)
        {
            _logger.LogWarning("[Warn] Attempted to delete non-existent album {AlbumId}", id);
            throw new KeyNotFoundException($"Album with ID {id} was not found.");
        }

        _db.Albums.Remove(albumEntity);
        await _db.SaveChangesAsync();
        _logger.LogInformation("[Info] Successfully deleted album {AlbumId}", id);
    }

    public async Task<List<AlbumInfoDto>> GetListOfNewestAsync()
    {
        var cutoffDate = DateTime.Now.AddMonths(-2);
        _logger.LogDebug("[Debug] Fetching newest albums (created before {CutoffDate})", cutoffDate);
        
        var albums = await _db.Albums.AsNoTracking()
            .Where(x => x.CreatedAt < cutoffDate)
            .ToListAsync();
            
        _logger.LogInformation("[Info] Retrieved {Count} newest albums", albums.Count);
        return _mapper.Map<List<AlbumInfoDto>>(albums);
    }

    public async Task<List<AlbumInfoDto>> GetListByAlbumNameAsync(string search)
    {
        _logger.LogDebug("[Debug] Searching albums with pattern: {SearchPattern}", search);
        var albums = await _db.AlbumsAuthors.AsNoTracking()
            .Where(x => EF.Functions.Like(x.IdAlbumNavigation.Name, $"%{search}%"))
            .Select(x => x.IdAlbumNavigation)
            .Take(100)
            .ToListAsync();
            
        _logger.LogInformation("[Info] Found {Count} albums matching search '{SearchPattern}'", albums.Count, search);
        return _mapper.Map<List<AlbumInfoDto>>(albums);
    }

    public async Task<List<AlbumInfoDto>> GetListByUserIdAsync(long userId)
    {
        _logger.LogDebug("[Debug] Fetching albums for author {UserId}", userId);
        var albums = await _db.AlbumsAuthors.AsNoTracking()
            .Where(x => x.IdAuthor == userId)
            .Select(x => x.IdAlbumNavigation)
            .Take(100)
            .ToListAsync();
            
        _logger.LogInformation("[Info] Retrieved {Count} albums for author {UserId}", albums.Count, userId);
        return _mapper.Map<List<AlbumInfoDto>>(albums);
    }

    public async Task<List<AlbumInfoDto>> GetListSavedAsync(long userId)
    {
        _logger.LogDebug("[Debug] Fetching saved albums for user {UserId}", userId);
        var albums = await _db.SavedAlbums.AsNoTracking()
            .Where(x => x.IdUser == userId)
            .Select(x => x.IdAlbumNavigation)
            .Take(100)
            .ToListAsync();
            
        _logger.LogInformation("[Info] Retrieved {Count} saved albums for user {UserId}", albums.Count, userId);
        return _mapper.Map<List<AlbumInfoDto>>(albums);
    }

    public async Task SaveAlbumAsync(long idUser, long idAlbum)
    {
        _logger.LogDebug("[Debug] Attempting to save album {AlbumId} for user {UserId}", idAlbum, idUser);
        
        var existing = await _db.SavedAlbums.AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdAlbum == idAlbum && x.IdUser == idUser);

        if (existing is not null)
        {
            _logger.LogWarning("[Warn] User {UserId} already has album {AlbumId} saved", idUser, idAlbum);
            throw new InvalidOperationException("Album is already saved.");
        }

        var savedAlbum = new SavedAlbum { IdAlbum = idAlbum, IdUser = idUser };
        await _db.SavedAlbums.AddAsync(savedAlbum);
        await _db.SaveChangesAsync();
        _logger.LogInformation("[Info] Successfully saved album {AlbumId} for user {UserId}", idAlbum, idUser);
    }

    public async Task UnsaveAlbumAsync(long idUser, long idAlbum)
    {
        _logger.LogDebug("[Debug] Attempting to unsave album {AlbumId} for user {UserId}", idAlbum, idUser);
        
        var savedAlbum = await _db.SavedAlbums.FirstOrDefaultAsync(x => 
            x.IdAlbum == idAlbum && x.IdUser == idUser);

        if (savedAlbum is null)
        {
            _logger.LogWarning("[Warn] User {UserId} does not have album {AlbumId} saved", idUser, idAlbum);
            throw new InvalidOperationException("Album is not saved.");
        }

        _db.SavedAlbums.Remove(savedAlbum);
        await _db.SaveChangesAsync();
        _logger.LogInformation("[Info] Successfully unsaved album {AlbumId} for user {UserId}", idAlbum, idUser);
    }
}