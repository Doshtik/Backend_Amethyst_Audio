using System.Data;
using System.Text.Json;
using AutoMapper;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models.Data;
using Backend_Amethyst_Audio.Models.Entities;
using Backend_Amethyst_Audio.Models.Enums;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Backend_Amethyst_Audio.Services.Implementations;

public class PlaylistService : IPlaylistService
{
    private readonly AppDbContext _db;
    private readonly IMediaService _mediaService;
    private readonly IMapper _mapper;
    private readonly ILogger<PlaylistService> _logger;
    private IPlaylistService _playlistServiceImplementation;

    public PlaylistService(AppDbContext db, IMediaService mediaService, IMapper mapper, ILogger<PlaylistService> logger)
    {
        _db = db;
        _mediaService = mediaService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PlaylistInfoDto> GetByIdAsync(long id, string requestedBy = null)
    {
        _logger.LogDebug("[Debug] Get playlist by Id. PlaylistId={PlaylistId}, RequestedBy={UserId}", 
            id, requestedBy ?? "anonymous");
        
        Playlist? playlist = await _db.Playlists
            .AsNoTracking()
            .Include(x => x.PlaylistsTracks)
            .ThenInclude(x => x.IdTrackNavigation)
            .ThenInclude(x => x.TracksAuthors)
            .ThenInclude(x => x.IdAuthorNavigation)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (playlist == null)
        {
            _logger.LogWarning("[Warn] Playlist not found. PlaylistId={PlaylistId}", id);
            throw new KeyNotFoundException("Playlist not found");
        }
        
        _logger.LogDebug("[Debug] Playlist retrieved. PlaylistId={PlaylistId}, OwnerId={OwnerId}", 
            id, playlist.IdUser);
        
        return _mapper.Map<PlaylistInfoDto>(playlist);
    }

    public async Task<List<PlaylistInfoDto>> GetAllAsync()
    {
        _logger.LogDebug("[Debug] Get all playlists request");
        
        var playlists = await _db.Playlists
            .AsNoTracking()
            .Include(x => x.PlaylistsTracks)
            .ThenInclude(x => x.IdTrackNavigation)
            .ThenInclude(x => x.TracksAuthors)
            .ThenInclude(x => x.IdAuthorNavigation)
            .ToListAsync();
        
        var result = _mapper.Map<List<PlaylistInfoDto>>(playlists);
        
        _logger.LogInformation("[Info] Retrieved {Count} playlists", result.Count);
        return result;
    }

    public async Task<PlaylistInfoDto> CreateAsync(CreatePlaylistDto dto, long ownerId)
    {
        List<long> tracksIds = JsonSerializer.Deserialize<List<long>>(dto.TracksIdList) 
            ?? throw new NoNullAllowedException("Playlist must have tracks");
        
        _logger.LogInformation("[Info] Creating playlist. OwnerId={OwnerId}, Title={Title}, IsPublic={IsPublic}, TrackCount={TrackCount}", 
            ownerId, dto.Name, dto.IsPublic, tracksIds.Count);
        
        // Playlist name validation
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            _logger.LogWarning("[Warn] Playlist creation failed: empty name. OwnerId={OwnerId}", ownerId);
            throw new ArgumentException("Playlist name cannot be empty");
        }
        
        // Playlist name length validation
        if (dto.Name.Length > 200)
        {
            _logger.LogWarning("[Warn] Playlist name too long. Length={Length}", dto.Name.Length);
            throw new ArgumentException("Playlist name is too long");
        }
        
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
            coverFileName = "default-cover.jpg";
            _logger.LogDebug("[Debug] No cover file provided, using default: {FileName}", coverFileName);
        }

        Playlist playlist = _mapper.Map<Playlist>(dto);
        playlist.IdUser = ownerId;
        playlist.CoverFileName = coverFileName;
        playlist.CreatedAt = DateTime.UtcNow;
        playlist.UpdatedAt = DateTime.UtcNow;
        
        await _db.Playlists.AddAsync(playlist);
        await _db.SaveChangesAsync();

        _logger.LogDebug("[Debug] Saving playlist tracks PlaylistId={PlaylistId}", playlist.Id);
        foreach (long trackId in tracksIds)
        {
            _db.PlaylistsTracks.Add(new PlaylistsTrack
            {
                IdTrack = trackId, 
                IdPlaylist = playlist.Id, 
                CreatedAt = DateTime.UtcNow
            });
        }
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("[Info] Playlist created successfully. PlaylistId={PlaylistId}, Title={Title}", 
            playlist.Id, playlist.Name);
        
        return _mapper.Map<PlaylistInfoDto>(playlist);
    }

    public async Task<PlaylistInfoDto> UpdateAsync(long id, ChangePlaylistInfoDto dto, string requestedBy)
    {
        _logger.LogInformation("[Info] Update playlist request. PlaylistId={PlaylistId}, UserId={UserId}", 
            id, requestedBy);
        
        List<long> addedTracksIdList = JsonSerializer.Deserialize<List<long>>(dto.AddedTracksIdList) ?? new List<long>();
        List<long> removedTracksIdList = JsonSerializer.Deserialize<List<long>>(dto.RemovedTracksIdList) ?? new List<long>();
        
        var playlist = await _db.Playlists.FirstOrDefaultAsync(p => p.Id == id);
        
        if (playlist == null)
        {
            _logger.LogWarning("[Warn] Playlist not found for update. PlaylistId={PlaylistId}", id);
            throw new KeyNotFoundException("Playlist not found");
        }
        
        // Rights validation: only owner can update playlist
        if (playlist.IdUser.ToString() != requestedBy)
        {
            _logger.LogWarning("[Warn] Unauthorized update attempt. PlaylistId={PlaylistId}, OwnerId={OwnerId}, RequestedBy={UserId}", 
                id, playlist.IdUser, requestedBy);
            throw new UnauthorizedAccessException("You can only edit your own playlists");
        }
        
        // Name validation after update
        if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            if (dto.Name.Length > 200)
            {
                _logger.LogWarning("[Warn] New playlist name too long. Length={Length}", dto.Name.Length);
                throw new ArgumentException("Playlist name is too long");
            }
            playlist.Name = dto.Name;
        }
        
        if (addedTracksIdList.Count > 0)
        {
            foreach (long trackId in addedTracksIdList)
            {
                if (!_db.PlaylistsTracks.Any(t => 
                        t.IdTrack == trackId &&
                        t.IdPlaylist == id))
                {
                    playlist.PlaylistsTracks.Add(new PlaylistsTrack 
                    { 
                        IdTrack = trackId, 
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }
        
        if (removedTracksIdList.Count > 0)
        {
            foreach (long trackId in removedTracksIdList)
            {
                Track? trackItem = await _db.Tracks.FirstOrDefaultAsync(t => t.Id == trackId);
                
                if (trackItem is null)
                {
                    _logger.LogWarning("[Warn] Track not found for remove. PlaylistId={PlaylistId}, TrackId={TrackId}", playlist.Id, trackId);
                    continue;
                }
                
                PlaylistsTrack? track = await _db.PlaylistsTracks
                    .Where(x => x.IdTrack == trackItem.Id && x.IdPlaylist == id)
                    .FirstOrDefaultAsync();
                
                if (track is not null)
                {
                    playlist.PlaylistsTracks.Remove(track);
                }
            }
        }
        
        if (dto.Description != null)
            playlist.Description = dto.Description;
        
        if (dto.IsPublic.HasValue)
            playlist.IsPublic = (bool)dto.IsPublic;
        
        playlist.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        if (playlist.CreatedAt.Kind != DateTimeKind.Utc)
        {
            playlist.CreatedAt = DateTime.SpecifyKind(playlist.CreatedAt, DateTimeKind.Utc);
        }
        
        _db.Playlists.Update(playlist);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("[Info] Playlist updated successfully. PlaylistId={PlaylistId}", id);
        return _mapper.Map<PlaylistInfoDto>(playlist);
    }

    public async Task DeleteAsync(long id, string requestedBy)
    {
        _logger.LogInformation("[Info] Delete playlist request. PlaylistId={PlaylistId}, UserId={UserId}", 
            id, requestedBy);
        
        var playlist = await _db.Playlists.FirstOrDefaultAsync(p => p.Id == id);
        
        if (playlist == null)
        {
            _logger.LogWarning("[Warn] Playlist not found for deletion. PlaylistId={PlaylistId}", id);
            throw new KeyNotFoundException("Playlist not found");
        }
        
        // Rights validation: only owner can delete playlist
        if (playlist.IdUser.ToString() != requestedBy)
        {
            _logger.LogWarning("[Warn] Unauthorized delete attempt. PlaylistId={PlaylistId}, OwnerId={OwnerId}, RequestedBy={UserId}", 
                id, playlist.IdUser, requestedBy);
            throw new UnauthorizedAccessException("You can only delete your own playlists");
        }
        
        // Delete linked records (tracks, saves)
        await _db.PlaylistsTracks.Where(pt => pt.IdPlaylist == id).ExecuteDeleteAsync();
        await _db.SavedPlaylists.Where(sp => sp.IdPlaylist == id).ExecuteDeleteAsync();
        
        _db.Playlists.Remove(playlist);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("[Info] Playlist deleted successfully. PlaylistId={PlaylistId}", id);
    }

    public async Task<List<PlaylistInfoDto>> GetListByUserIdAsync(long userId)
    {
        _logger.LogDebug("[Debug] Get playlists by owner. OwnerId={OwnerId}", userId);
        
        var playlists = await _db.Playlists
            .AsNoTracking()
            .Include(x => x.PlaylistsTracks)
            .ThenInclude(x => x.IdTrackNavigation)
            .ThenInclude(x => x.TracksAuthors)
            .ThenInclude(x => x.IdAuthorNavigation)
            .Where(x => x.IdUser == userId)
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync();
        
        var result = _mapper.Map<List<PlaylistInfoDto>>(playlists);
        
        _logger.LogInformation("[Info] Retrieved {Count} playlists for owner. OwnerId={OwnerId}", 
            result.Count, userId);
        return result;
    }

    public async Task<List<PlaylistInfoDto>> GetListByPlaylistNameAsync(long excludeUserId, string query, int limit = 50)
    {
        _logger.LogDebug("[Debug] Search public playlists. Query={Query}, ExcludeUserId={UserId}, Limit={Limit}", 
            query, excludeUserId, limit);
        
        if (string.IsNullOrWhiteSpace(query))
            return new List<PlaylistInfoDto>();
        
        List<Playlist> playlists = await _db.Playlists
            .AsNoTracking()
            .Where(x => 
                EF.Functions.ILike(x.Name, $"%{query}%") &&
                x.IsPublic &&
                x.IdUser != excludeUserId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(limit)
            .ToListAsync();
        
        var result = _mapper.Map<List<PlaylistInfoDto>>(playlists);
        
        _logger.LogInformation("[Info] Search completed. Query={Query}, Found={Count}", query, result.Count);
        return result;
    }

    public async Task<List<PlaylistInfoDto>> GetListSavedAsync(long userId)
    {
        _logger.LogDebug("[Debug] Get saved playlists for user. UserId={UserId}", userId);
        
        List<Playlist> playlists = await _db.SavedPlaylists
            .AsNoTracking()
            .Include(x => x.IdPlaylistNavigation)
            .ThenInclude(x => x.PlaylistsTracks)
            .ThenInclude(x => x.IdTrackNavigation)
            .ThenInclude(x => x.TracksAuthors)
            .ThenInclude(x => x.IdAuthorNavigation)
            .Where(x => x.IdUser == userId)
            .Select(x => x.IdPlaylistNavigation)
            .ToListAsync();
        
        var result = _mapper.Map<List<PlaylistInfoDto>>(playlists);
        
        _logger.LogInformation("[Info] Retrieved {Count} saved playlists. UserId={UserId}", 
            result.Count, userId);
        return result;
    }

    public async Task AddInPlaylistAsync(long trackId, long playlistId)
    {
        _logger.LogInformation("[Info] Add track to playlist. TrackId={TrackId}, PlaylistId={PlaylistId}", 
            trackId, playlistId);
        
        bool isTrackExists = await _db.Tracks.AnyAsync(t => t.Id == trackId);
        if (!isTrackExists)
        {
            _logger.LogWarning("[Warn] Track not found for playlist addition. TrackId={TrackId}", trackId);
            throw new KeyNotFoundException("Track not found");
        }
        
        bool isPlaylistExists = await _db.Playlists.AnyAsync(p => p.Id == playlistId);
        if (!isPlaylistExists)
        {
            _logger.LogWarning("[Warn] Playlist not found for track addition. PlaylistId={PlaylistId}", playlistId);
            throw new KeyNotFoundException("Playlist not found");
        }
        
        bool isExisting = await _db.PlaylistsTracks
            .AnyAsync(x => x.IdTrack == trackId && x.IdPlaylist == playlistId);
        
        if (isExisting)
        {
            _logger.LogWarning("[Warn] Track already in playlist. TrackId={TrackId}, PlaylistId={PlaylistId}", 
                trackId, playlistId);
            throw new InvalidOperationException("Track already exists in playlist");
        }

        var playlistTrack = new PlaylistsTrack
        {
            IdTrack = trackId,
            IdPlaylist = playlistId,
            CreatedAt = DateTime.UtcNow
        };
        
        await _db.PlaylistsTracks.AddAsync(playlistTrack);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("[Info] Track added to playlist. TrackId={TrackId}, PlaylistId={PlaylistId}", 
            trackId, playlistId);
    }

    public async Task RemoveFromPlaylistAsync(long trackId, long playlistId)
    {
        _logger.LogInformation("[Info] Remove track from playlist. TrackId={TrackId}, PlaylistId={PlaylistId}", 
            trackId, playlistId);
        
        PlaylistsTrack? playlistTrack = await _db.PlaylistsTracks
            .FirstOrDefaultAsync(x => x.IdTrack == trackId && x.IdPlaylist == playlistId);
        
        if (playlistTrack == null)
        {
            _logger.LogWarning("[Warn] Track not found in playlist. TrackId={TrackId}, PlaylistId={PlaylistId}", 
                trackId, playlistId);
            throw new KeyNotFoundException("Track not found in playlist");
        }
        
        _db.PlaylistsTracks.Remove(playlistTrack);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("[Info] Track removed from playlist. TrackId={TrackId}, PlaylistId={PlaylistId}", 
            trackId, playlistId);
    }

    public async Task<bool> IsPlaylistSavedAsync(long userId, long playlistId)
    {
        bool isPlaylistSaved = await _db.SavedPlaylists.AsNoTracking()
            .AnyAsync(p => p.IdUser == userId && p.IdPlaylist == playlistId);
        return isPlaylistSaved;
    }

    public async Task SavePlaylistAsync(long userId, long playlistId)
    {
        _logger.LogInformation("[Info] Save playlist request. UserId={UserId}, PlaylistId={PlaylistId}", 
            userId, playlistId);
        
        // Exist validation
        var playlistExists = await _db.Playlists.AnyAsync(p => p.Id == playlistId);
        if (!playlistExists)
        {
            _logger.LogWarning("[Warn] Playlist not found for save. PlaylistId={PlaylistId}", playlistId);
            throw new KeyNotFoundException("Playlist not found");
        }
        
        // Check: user can't save his own playlist
        if (userId == await _db.Playlists.Where(p => p.Id == playlistId).Select(p => p.IdUser).FirstOrDefaultAsync())
        {
            _logger.LogWarning("[Warn] User attempted to save own playlist. UserId={UserId}, PlaylistId={PlaylistId}", 
                userId, playlistId);
            throw new InvalidOperationException("Cannot save your own playlist");
        }
        
        // Duplicate validation
        var existing = await _db.SavedPlaylists
            .AnyAsync(p => p.IdPlaylist == playlistId && p.IdUser == userId);
        
        if (existing)
        {
            _logger.LogWarning("[Warn] Playlist already saved. UserId={UserId}, PlaylistId={PlaylistId}", 
                userId, playlistId);
            throw new InvalidOperationException("Playlist already saved");
        }

        var savedPlaylist = new SavedPlaylist
        {
            IdUser = userId,
            IdPlaylist = playlistId
        };
        
        await _db.SavedPlaylists.AddAsync(savedPlaylist);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("[Info] Playlist saved successfully. UserId={UserId}, PlaylistId={PlaylistId}", 
            userId, playlistId);
    }

    public async Task UnsavePlaylistAsync(long userId, long playlistId)
    {
        _logger.LogInformation("[Info] Unsave playlist request. UserId={UserId}, PlaylistId={PlaylistId}", 
            userId, playlistId);
        
       SavedPlaylist? savedPlaylist = await _db.SavedPlaylists
            .FirstOrDefaultAsync(p => p.IdPlaylist == playlistId && p.IdUser == userId);
        
        if (savedPlaylist == null)
        {
            _logger.LogWarning("[Warn] Saved playlist not found. UserId={UserId}, PlaylistId={PlaylistId}", 
                userId, playlistId);
            throw new InvalidOperationException("Playlist not in saved list");
        }
        
        _db.SavedPlaylists.Remove(savedPlaylist);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("[Info] Playlist unsaved successfully. UserId={UserId}, PlaylistId={PlaylistId}", 
            userId, playlistId);
    }
}