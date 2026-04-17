using AutoMapper;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models.Data;
using Backend_Amethyst_Audio.Models.Entities;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Backend_Amethyst_Audio.Services.Implementations;

public class PlaylistService : IPlaylistService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly ILogger<PlaylistService> _logger;
    
    public PlaylistService(AppDbContext db, IMapper mapper, ILogger<PlaylistService> logger)
    {
        _db = db;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PlaylistInfoDto> GetByIdAsync(long id, string requestedBy = null)
    {
        _logger.LogDebug("[Debug] Get playlist by Id. PlaylistId={PlaylistId}, RequestedBy={UserId}", 
            id, requestedBy ?? "anonymous");
        
        Playlist? playlist = await _db.Playlists
            .AsNoTracking()
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
            .ToListAsync();
        
        var result = _mapper.Map<List<PlaylistInfoDto>>(playlists);
        
        _logger.LogInformation("[Info] Retrieved {Count} playlists", result.Count);
        return result;
    }

    public async Task<PlaylistInfoDto> CreateAsync(CreatePlaylistDto dto, long ownerId)
    {
        _logger.LogInformation("[Info] Creating playlist. OwnerId={OwnerId}, Title={Title}, IsPublic={IsPublic}", 
            ownerId, dto.Name, dto.AccessTypeName);
        
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

        var playlist = _mapper.Map<Playlist>(dto);
        playlist.IdUser = ownerId;
        playlist.CreatedAt = DateTime.UtcNow;
        playlist.UpdatedAt = DateTime.UtcNow;
        
        await _db.Playlists.AddAsync(playlist);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("[Info] Playlist created successfully. PlaylistId={PlaylistId}, Title={Title}", 
            playlist.Id, playlist.Name);
        
        return _mapper.Map<PlaylistInfoDto>(playlist);
    }

    public async Task<PlaylistInfoDto> UpdateAsync(long id, ChangePlaylistInfoDto dto, string requestedBy)
    {
        _logger.LogInformation("[Info] Update playlist request. PlaylistId={PlaylistId}, UserId={UserId}", 
            id, requestedBy);
        
        var playlist = await _db.Playlists.FirstOrDefaultAsync(p => p.Id == id);
        
        if (playlist == null)
        {
            _logger.LogWarning("[Warn] Playlist not found for update. PlaylistId={PlaylistId}", id);
            throw new KeyNotFoundException("Playlist not found");
        }
        
        // Rights validation: only owner can update playlist
        if (playlist.IdUser.ToString() != requestedBy && !IsAdmin(requestedBy))
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
        
        if (dto.Description != null)
            playlist.Description = dto.Description;
        
        playlist.IsPublic = dto.IsPublic;
        playlist.UpdatedAt = DateTime.UtcNow;
        
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
        if (playlist.IdUser.ToString() != requestedBy && !IsAdmin(requestedBy))
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
                EF.Functions.Like(x.Name, $"%{query}%") &&
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
    
    // Helper: проверка на админа (заглушка)
    private bool IsAdmin(string userId)
    {
        // TODO: Реализовать проверку роли через _userManager.IsInRoleAsync или кэш
        return false;
    }
}