using System.Data;
using System.Text.Json;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        var album = await _db.Albums
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (album is null)
        {
            _logger.LogWarning("[Warn] Album with ID {AlbumId} was not found", id);
            throw new KeyNotFoundException($"Album with ID {id} was not found.");
        }

        _logger.LogDebug("[Debug] Successfully fetched album {AlbumId}", id);
        AlbumInfoDto resultDto = _mapper.Map<AlbumInfoDto>(album);
        resultDto.TrackList = _mapper.Map<List<TrackInfoDto>>(await _db.AlbumsTracks
            .AsNoTracking()
            .Include(x => x.IdTrackNavigation)
            .ThenInclude(x => x.TracksAuthors)
            .ThenInclude(x => x.IdAuthorNavigation)
            .Where(x => x.IdAlbum == album.Id)
            .Select(x => x.IdTrackNavigation)
            .ToListAsync());
        resultDto.AuthorList = _mapper.Map<List<UserInfoDto>>(await _db.AlbumsAuthors
            .AsNoTracking()
            .Where(x => x.IdAlbum == album.Id)
            .Select(x => x.IdAuthorNavigation)
            .ToListAsync());
        return resultDto;
    }

    public async Task<List<AlbumInfoDto>> GetAllAsync()
    {
        _logger.LogDebug("[Debug] Fetching all albums");
        var albums = await _db.Albums
            .AsNoTracking()
            .ToListAsync();
        _logger.LogInformation("[Info] Retrieved {Count} albums", albums.Count);
        List<AlbumInfoDto> resultDto = _mapper.Map<List<AlbumInfoDto>>(albums);
        foreach (var album in resultDto)
        {
            album.TrackList = _mapper.Map<List<TrackInfoDto>>(await _db.AlbumsTracks
                .AsNoTracking()
                .Include(x => x.IdTrackNavigation)
                .ThenInclude(x => x.TracksAuthors)
                .ThenInclude(x => x.IdAuthorNavigation)
                .Where(x => x.IdAlbum == album.Id)
                .Select(x => x.IdTrackNavigation)
                .ToListAsync());
            album.AuthorList = _mapper.Map<List<UserInfoDto>>(await _db.AlbumsAuthors
                .AsNoTracking()
                .Where(x => x.IdAlbum == album.Id)
                .Select(x => x.IdAuthorNavigation)
                .ToListAsync());
        }
        return resultDto;
    }

    public async Task<AlbumInfoDto> CreateAsync(long userId, CreateAlbumDto dto)
    {
        _logger.LogDebug("[Debug] Creating new album from DTO: {Dto}", dto);
        
        string coverFileName;
        if (dto.CoverFile is not null)
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
            _logger.LogError("[Debug] No cover file provided");
            throw new NoNullAllowedException("Cover is required.");
        }
        
        var authorIds = JsonSerializer.Deserialize<List<long>>(dto.AuthorsIdList);
        var trackIds = JsonSerializer.Deserialize<List<long>>(dto.TracksIdList);

        var existingAuthors = await _db.Users
            .Where(u => authorIds.Contains(u.Id))
            .Select(u => u.Id)
            .ToHashSetAsync();
    
        var missingAuthors = authorIds.Except(existingAuthors);
        if (missingAuthors.Any())
            throw new KeyNotFoundException($"Authors doesn't found: {string.Join(", ", missingAuthors)}");

        var existingTracks = await _db.Tracks
            .Where(t => trackIds.Contains(t.Id))
            .Select(t => t.Id)
            .ToHashSetAsync();
    
        var missingTracks = trackIds.Except(existingTracks);
        if (missingTracks.Any())
            throw new KeyNotFoundException($"Tracks doesn't found: {string.Join(", ", missingTracks)}");

        var albumEntity = _mapper.Map<Album>(dto);
        albumEntity.CoverFileName = coverFileName;
        albumEntity.CreatedAt = DateTime.UtcNow;
        albumEntity.UpdatedAt = DateTime.UtcNow;

        await _db.Albums.AddAsync(albumEntity);
        await _db.SaveChangesAsync();

        foreach (var authorId in authorIds)
        {
            await _db.AlbumsAuthors.AddAsync(new AlbumsAuthor 
            { 
                IdAlbum = albumEntity.Id, 
                IdAuthor = authorId
            });
        }

        foreach (var trackId in trackIds)
        {
            await _db.AlbumsTracks.AddAsync(new AlbumsTrack 
            { 
                IdAlbum = albumEntity.Id, 
                IdTrack = trackId,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _db.SaveChangesAsync();
        
        _logger.LogInformation("[Info] Successfully created album with ID {AlbumId}", albumEntity.Id);
        return await GetAlbumInfoDtoAsync(albumEntity.Id);
    }

    public async Task<AlbumInfoDto> UpdateAsync(long albumId, ChangeAlbumInfoDto dto)
    {
        var albumEntity = await _db.Albums
            .FirstOrDefaultAsync(a => a.Id == albumId)
            ?? throw new KeyNotFoundException($"Album with ID {albumId} was not found");

        if (!string.IsNullOrWhiteSpace(dto.Name))
            albumEntity.Name = dto.Name;
        
        albumEntity.UpdatedAt = DateTime.UtcNow;

        if (dto.CoverFile != null)
        {
            if (!string.IsNullOrEmpty(albumEntity.CoverFileName) && 
                albumEntity.CoverFileName != "default-cover.jpg")
            {
                await _mediaService.DeleteFileAsync(albumEntity.CoverFileName, FileTypes.Covers);
            }
            
            albumEntity.CoverFileName = await _mediaService.SaveFileAsync(dto.CoverFile, FileTypes.Covers);
        }
        
        await _db.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(dto.AddedTrackList))
        {
            var addedTrackIds = JsonSerializer.Deserialize<List<long>>(dto.AddedTrackList);
            
            var existingTracks = await _db.Tracks
                .Where(t => addedTrackIds.Contains(t.Id))
                .Select(t => t.Id)
                .ToHashSetAsync();
            
            var missingTracks = addedTrackIds.Except(existingTracks);
            if (missingTracks.Any())
                throw new KeyNotFoundException($"Tracks doesn't found: {string.Join(", ", missingTracks)}");

            var existingAlbumTrackIds = await _db.AlbumsTracks
                .Where(at => at.IdAlbum == albumEntity.Id)
                .Select(at => at.IdTrack)
                .ToHashSetAsync();

            foreach (var trackId in addedTrackIds)
            {
                if (!existingAlbumTrackIds.Contains(trackId))
                {
                    await _db.AlbumsTracks.AddAsync(new AlbumsTrack
                    {
                        IdAlbum = albumEntity.Id,
                        IdTrack = trackId,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        // 6. Обработка удалённых треков
        if (!string.IsNullOrWhiteSpace(dto.RemovedTrackList))
        {
            var removedTrackIds = JsonSerializer.Deserialize<List<long>>(dto.RemovedTrackList);
            
            var tracksToRemove = await _db.AlbumsTracks
                .Where(at => at.IdAlbum == albumEntity.Id && removedTrackIds.Contains(at.IdTrack))
                .ToListAsync();
            
            _db.AlbumsTracks.RemoveRange(tracksToRemove);
        }

        // 7. Финальное сохранение изменений связей
        await _db.SaveChangesAsync();

        // 8. Формируем ответ
        _logger.LogInformation("[Info] Successfully updated album {AlbumId}", albumEntity.Id);
        return await GetAlbumInfoDtoAsync(albumEntity.Id);
    }

    // Хелпер для формирования DTO (чтобы не дублировать код из CreateAsync)
    private async Task<AlbumInfoDto> GetAlbumInfoDtoAsync(long albumId)
    {
        var album = await _db.Albums.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == albumId)
            ?? throw new KeyNotFoundException($"Album {albumId} has not been found");
    
        var resultDto = _mapper.Map<AlbumInfoDto>(album);
    
        resultDto.TrackList = await _db.AlbumsTracks.AsNoTracking()
            .Where(x => x.IdAlbum == albumId)
            .Select(x => x.IdTrackNavigation)
            .ProjectTo<TrackInfoDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        
        resultDto.AuthorList = await _db.AlbumsAuthors.AsNoTracking()
            .Where(x => x.IdAlbum == albumId)
            .Select(x => x.IdAuthorNavigation)
            .ProjectTo<UserInfoDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    
        return resultDto;
    }

    public async Task DeleteAsync(long id)
    {
        _logger.LogDebug("[Debug] Attempting to delete album {AlbumId}", id);
        
        var albumEntity = await _db.Albums.FindAsync(id);
        
        var tracksInAlbum = _db.AlbumsTracks
            .Where(x => x.IdAlbum == id);
        
        var usersInAlbum = _db.AlbumsAuthors
            .Where(x => x.IdAlbum == id);

        if (albumEntity is null)
        {
            _logger.LogWarning("[Warn] Attempted to delete non-existent album {AlbumId}", id);
            throw new KeyNotFoundException($"Album with ID {id} was not found.");
        }

        try
        {
            if (!string.IsNullOrEmpty(albumEntity.CoverFileName))
            {
                await _mediaService.DeleteFileAsync(albumEntity.CoverFileName, FileTypes.Covers);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to delete album cover. {AlbumId}", id);
            throw new Exception($"Failed to delete album cover.");
        }
        

        _db.AlbumsTracks.RemoveRange(tracksInAlbum);
        _db.AlbumsAuthors.RemoveRange(usersInAlbum);
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
        var albums = await _db.AlbumsAuthors
            .AsNoTracking()
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
        var albums = await _db.AlbumsAuthors
            .AsNoTracking()
            .Where(x => x.IdAuthor == userId)
            .Select(x => x.IdAlbumNavigation)
            .Take(100)
            .ToListAsync();
            
        _logger.LogInformation("[Info] Retrieved {Count} albums for author {UserId}", albums.Count, userId);
        List<AlbumInfoDto> resultDto = _mapper.Map<List<AlbumInfoDto>>(albums);
        foreach (var album in resultDto)
        {
            album.TrackList = _mapper.Map<List<TrackInfoDto>>(await _db.AlbumsTracks
                .AsNoTracking()
                .Include(x => x.IdTrackNavigation)
                .ThenInclude(x => x.TracksAuthors)
                .ThenInclude(x => x.IdAuthorNavigation)
                .Where(x => x.IdAlbum == album.Id)
                .Select(x => x.IdTrackNavigation)
                .ToListAsync());
            album.AuthorList = _mapper.Map<List<UserInfoDto>>(await _db.AlbumsAuthors
                .AsNoTracking()
                .Where(x => x.IdAlbum == album.Id)
                .Select(x => x.IdAuthorNavigation)
                .ToListAsync());
        }
        return resultDto;
    }

    public async Task<List<AlbumInfoDto>> GetListSavedAsync(long userId)
    {
        _logger.LogDebug("[Debug] Fetching saved albums for user {UserId}", userId);
        var albums = await _db.SavedAlbums
            .AsNoTracking()
            .Include(x => x.IdAlbumNavigation)
            .ThenInclude(x => x.AlbumsAuthors)
            .ThenInclude(x => x.IdAuthorNavigation)
            .Include(x => x.IdAlbumNavigation)
            .ThenInclude(x => x.AlbumsTracks)
            .ThenInclude(x => x.IdTrackNavigation)
            .ThenInclude(x => x.TracksAuthors)
            .ThenInclude(x => x.IdAuthorNavigation)
            .Where(x => x.IdUser == userId)
            .Select(x => x.IdAlbumNavigation)
            .Take(100)
            .ToListAsync();
            
        _logger.LogInformation("[Info] Retrieved {Count} saved albums for user {UserId}", albums.Count, userId);
        return _mapper.Map<List<AlbumInfoDto>>(albums);
    }

    public async Task<bool> IsAlbumSavedAsync(long userId, long albumId)
    {
        bool isAlbumSaved = await _db.SavedAlbums
            .AsNoTracking()
            .AnyAsync(x => x.IdAlbum == albumId && x.IdUser == userId);

        return isAlbumSaved;
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