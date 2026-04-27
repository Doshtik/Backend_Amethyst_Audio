using System.Text.Json;
using AutoMapper;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.DTO.Pages;
using Backend_Amethyst_Audio.Models.Data;
using Backend_Amethyst_Audio.Models.Entities;
using Backend_Amethyst_Audio.Models.Enums;
using Backend_Amethyst_Audio.Profiles;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_Amethyst_Audio.Services.Implementations;

public class TracksService : ITrackService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly ILogger<TracksService> _logger;
    private readonly IMediaService _mediaService;
    
    public TracksService(
        AppDbContext db, 
        IMapper mapper, 
        ILogger<TracksService> logger,
        IMediaService mediaService)
    {
        _db = db;
        _mapper = mapper;
        _logger = logger;
        _mediaService = mediaService;
    }

    public async Task<TrackInfoDto> GetByIdAsync(long id)
    {
        _logger.LogDebug("[Debug] Fetching track by ID: {TrackId}", id);
        
        var track = await _db.Tracks
            .AsNoTracking()
            .Include(p => p.PlaylistsTracks)
            .Include(p => p.TracksAuthors)
            .ThenInclude(ta => ta.IdAuthorNavigation)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (track is null)
        {
            _logger.LogWarning("[Warn] Track not found. TrackId={TrackId}", id);
            throw new KeyNotFoundException("Track not found");
        }

        _logger.LogDebug("[Debug] Track retrieved successfully. TrackId={TrackId}", id);
        return _mapper.Map<TrackInfoDto>(track);
    }

    public async Task<List<TrackInfoDto>> GetAllAsync()
    {
        _logger.LogDebug("[Debug] Fetching all tracks");
        
        var tracks = await _db.Tracks
            .AsNoTracking()
            .Include(p => p.PlaylistsTracks)
            .Include(p => p.TracksAuthors)
            .ThenInclude(ta => ta.IdAuthorNavigation)
            .ToListAsync();
        
        _logger.LogInformation("[Info] Retrieved {Count} tracks", tracks.Count);
        return _mapper.Map<List<TrackInfoDto>>(tracks);
    }

    public async Task<TrackInfoDto> CreateAsync(CreateTrackDto dto, long userId)
    {
        List<long> authorIdList = JsonSerializer.Deserialize<List<long>>(dto.AuthorsIdList) ?? new List<long>();
        authorIdList.Add(userId);
        _logger.LogInformation("[Info] Creating new track. Title={Title}, AuthorsCount={AuthorsCount}", dto.Name, authorIdList?.Count ?? 0);

        string trackFileName, coverFileName;
        try
        {
            _logger.LogDebug("[Debug] Saving track audio file...");
            trackFileName = await _mediaService.SaveFileAsync(dto.TrackFile, FileTypes.Tracks);

            _logger.LogDebug("[Debug] Saving track cover file...");
            coverFileName = await _mediaService.SaveFileAsync(dto.CoverFile, FileTypes.Covers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Error] Failed to save media files for track '{Title}'", dto.Name);
            throw new InvalidOperationException("Failed to upload media files. Database record was not created.", ex);
        }

        var track = _mapper.Map<Track>(dto);

        Mood? mood = await _db.Moods.AsNoTracking().FirstOrDefaultAsync(x => x.MoodName == dto.MoodName);
        Pace? pace = await _db.Paces.AsNoTracking().FirstOrDefaultAsync(x => x.PaceName == dto.PaceName);
        
        track.IdMood = mood?.Id;
        track.IdPace = pace?.Id;
        track.CreatedAt = DateTime.UtcNow;
        track.UpdatedAt = DateTime.UtcNow;
        
        track.TrackFileName = trackFileName;
        track.CoverFileName = coverFileName;
        
        foreach (long usersId in authorIdList)
        {
            track.TracksAuthors.Add(new TracksAuthor
            {
                IdTrack = track.Id,
                IdAuthor = usersId
            });
        }
        
        await _db.Tracks.AddAsync(track);
        await _db.SaveChangesAsync();

        _logger.LogInformation("[Info] Track created successfully. TrackId={TrackId}, Title={Title}", track.Id, track.Name);
        
        var createdTrack = await _db.Tracks
            .AsNoTracking()
            .Include(t => t.TracksAuthors)
            .ThenInclude(ta => ta.IdAuthorNavigation)
            .FirstOrDefaultAsync(t => t.Id == track.Id);
        
        return _mapper.Map<TrackInfoDto>(createdTrack);
    }

    public async Task<TrackInfoDto> UpdateAsync(ChangeTrackInfoDto dto)
    {
        _logger.LogDebug("[Debug] Updating track. TrackId={TrackId}", dto.Id);

        var track = await _db.Tracks
            .Include(t => t.TracksAuthors)
                .ThenInclude(ta => ta.IdAuthorNavigation)
            .FirstOrDefaultAsync(t => t.Id == dto.Id);
        if (track == null)
        {
            _logger.LogWarning("[Warn] Track not found for update. TrackId={TrackId}", dto.Id);
            throw new KeyNotFoundException("Track not found");
        }

        _mapper.Map(dto, track);

        Mood? mood = await _db.Moods.AsNoTracking().FirstOrDefaultAsync(x => x.MoodName == dto.MoodName);
        Pace? pace = await _db.Paces.AsNoTracking().FirstOrDefaultAsync(x => x.PaceName == dto.PaceName);
        
        track.IdMood = mood?.Id;
        track.IdPace = pace?.Id;
        track.UpdatedAt = DateTime.UtcNow;

        if (dto.TrackFile is not null)
        {
            _logger.LogDebug("[Debug] Updating track audio file. OldFile={OldFile}", track.TrackFileName);
            try
            {
                var newFileName = await _mediaService.SaveFileAsync(dto.TrackFile, FileTypes.Tracks);
                
                if (!string.IsNullOrEmpty(track.TrackFileName))
                    await _mediaService.DeleteFileAsync(track.TrackFileName, FileTypes.Tracks);
                
                track.TrackFileName = newFileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Error] Failed to update track audio file. TrackId={TrackId}", track.Id);
                throw new InvalidOperationException("Failed to update audio file.", ex);
            }
        }

        if (dto.CoverFile is not null)
        {
            _logger.LogDebug("[Debug] Updating track cover file. OldFile={OldFile}", track.CoverFileName);
            try
            {

                var newFileName = await _mediaService.SaveFileAsync(dto.CoverFile, FileTypes.Covers);
                
                if (!string.IsNullOrEmpty(track.CoverFileName))
                    await _mediaService.DeleteFileAsync(track.CoverFileName, FileTypes.Covers);
                
                track.CoverFileName = newFileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Error] Failed to update track cover file. TrackId={TrackId}", track.Id);
                throw new InvalidOperationException("Failed to update cover file.", ex);
            }
        }

        _db.Tracks.Update(track);
        await _db.SaveChangesAsync();

        _logger.LogInformation("[Info] Track updated successfully. TrackId={TrackId}", track.Id);
        return _mapper.Map<TrackInfoDto>(track);
    }

    public async Task DeleteAsync(long id)
    {
        _logger.LogDebug("[Debug] Deleting track. TrackId={TrackId}", id);

        var track = await _db.Tracks.FirstOrDefaultAsync(t => t.Id == id);
        if (track is null)
        {
            _logger.LogWarning("[Warn] Track not found for deletion. TrackId={TrackId}", id);
            throw new KeyNotFoundException("Track not found");
        }

        try
        {
            if (!string.IsNullOrEmpty(track.TrackFileName))
                await _mediaService.DeleteFileAsync(track.TrackFileName, FileTypes.Tracks);
            
            if (!string.IsNullOrEmpty(track.CoverFileName))
                await _mediaService.DeleteFileAsync(track.CoverFileName, FileTypes.Covers);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[Warn] Failed to delete media files for track {TrackId}. Continuing with DB removal.", id);
        }

        _db.Tracks.Remove(track);
        await _db.SaveChangesAsync();
        _logger.LogInformation("[Info] Track deleted successfully. TrackId={TrackId}", id);
    }

    //TODO: Обязательно переделать. Это очень плохо
    public async Task<GenreInfoDto> GetListGenresAsync()
    {
        List<string> genres = await _db.Genres
            .Select(x => x.GenreName)
            .ToListAsync();
        GenreInfoDto dto = new GenreInfoDto()
        {
            GenreName = genres,
        };
        return dto;
    }

    public async Task<List<TrackInfoDto>> GetListByGenreAsync(string genre)
    {
        _logger.LogDebug("[Debug] Get tracks by genre. Genre={Genre}", genre);
        
        if (string.IsNullOrWhiteSpace(genre))
        {
            _logger.LogWarning("[Warn] Empty genre parameter in GetListByGenreAsync");
            throw new ArgumentException("Genre cannot be empty");
        }
        
        var tracks = await _db.TracksGenres
            .AsNoTracking()
            .Where(x => x.IdGenreNavigation.GenreName == genre)
            .Select(x => x.IdTrackNavigation)
            .ToListAsync();
        
        var result = _mapper.Map<List<TrackInfoDto>>(tracks);
        
        _logger.LogInformation("[Info] Retrieved {Count} tracks for genre={Genre}", result.Count, genre);
        return result;
    }

    public async Task<List<TrackInfoDto>> GetListByTrackNameAsync(string trackName)
    {
        _logger.LogDebug("[Debug] Search tracks by name. Query={Query}", trackName);
        
        if (string.IsNullOrWhiteSpace(trackName))
        {
            _logger.LogWarning("[Warn] Empty track name in search query");
            return new List<TrackInfoDto>();
        }
        
        // Поиск с частичным совпадением (case-insensitive)
        var tracks = await _db.Tracks
            .AsNoTracking()
            .Where(x => EF.Functions.Like(x.Name, $"%{trackName}%"))
            .Take(100)
            .ToListAsync();
        
        var result = _mapper.Map<List<TrackInfoDto>>(tracks);
        
        _logger.LogInformation("[Info] Search completed. Query={Query}, Found={Count}", trackName, result.Count);
        return result;
    }

    public async Task<List<TrackInfoDto>> GetListTrackByUserIdAsync(long userId)
    {
        _logger.LogDebug("[Debug] Get tracks by artist. ArtistId={ArtistId}", userId);
        
        var tracks = await _db.TracksAuthors
            .AsNoTracking()
            .Where(x => x.IdAuthor == userId)
            .Select(x => x.IdTrackNavigation)
            .ToListAsync();
        
        var result = _mapper.Map<List<TrackInfoDto>>(tracks);
        
        _logger.LogInformation("[Info] Retrieved {Count} tracks for artist. ArtistId={ArtistId}", 
            result.Count, userId);
        return result;
    }

    public async Task<List<TrackInfoDto>> GetListOfNewestAsync(int limit = 50)
    {
        _logger.LogDebug("[Debug] Get newest tracks. Limit={Limit}", limit);
        
        var thresholdDate = DateTime.UtcNow.AddDays(-60);
        
        var tracks = await _db.Tracks
            .AsNoTracking()
            .Where(x => x.CreatedAt >= thresholdDate)
            .OrderByDescending(x => x.CreatedAt)
            .Take(limit)
            .ToListAsync();
        
        var result = _mapper.Map<List<TrackInfoDto>>(tracks);
        
        _logger.LogInformation("[Info] Retrieved {Count} newest tracks. Since={Since}", 
            result.Count, thresholdDate.ToString("yyyy-MM-dd"));
        return result;
    }

    public async Task<List<TrackInfoDto>> GetUserLibraryAsync(long userId)
    {
        var tracks = await _db.Libraries
            .Where(x => x.IdUser == userId)
            .SelectMany(x => x.LibrariesTracks)
            .Select(x => x.IdTrackNavigation)
            .ToListAsync();
        return _mapper.Map<List<TrackInfoDto>>(tracks);
    }

    public async Task AddTrackToLibraryAsync(long trackId, long userId)
    {
        Library? library = await _db.Libraries.FirstOrDefaultAsync(x => x.IdUser == userId);

        if (library is null)
        {
            _logger.LogWarning("[Error] Library for {UserId} doesn't exist", userId);
            _logger.LogInformation("[Info] Creating a new library for {UserId}", userId);
            
            _db.Libraries.Add(new Library { IdUser = userId });
            await _db.SaveChangesAsync();
            
            _logger.LogInformation("[Info] Library for {UserId} created", userId);
            library = await _db.Libraries.FirstOrDefaultAsync(x => x.IdUser == userId);
        }
        
        Track? track = await _db.Tracks.FindAsync(trackId);
        
        if (track is null)
        {
            _logger.LogWarning("[Error] Track {TrackId} doesn't exist", trackId);
            throw new KeyNotFoundException("Track doesn't exist");
        }
        
        LibrariesTrack? libraryTrack = _db.LibrariesTracks.FirstOrDefault(x => 
            x.IdLibrary == library.Id &&
            x.IdTrack == trackId);
        
        if (libraryTrack is not null)
        {
            _logger.LogWarning("[Error] Library track {TrackId} already exist", trackId);
            throw new ArgumentException("Library track already exists");
        }
        
        _db.LibrariesTracks.Add(new LibrariesTrack
        {
            IdLibrary = library.Id, 
            IdTrack = track.Id, 
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        _logger.LogInformation("[Info] Track for {UserId} library was added", userId);
    }

    public async Task RemoveTrackToLibraryAsync(long trackId, long userId)
    {
        Library? library = await _db.Libraries.FirstOrDefaultAsync(x => x.IdUser == userId);

        if (library is null)
        {
            _logger.LogWarning("[Error] Library for {UserId} doesn't exist", userId);
            _logger.LogInformation("[Info] Creating a new library for {UserId}", userId);
            
            _db.Libraries.Add(new Library { IdUser = userId });
            await _db.SaveChangesAsync();
            
            _logger.LogInformation("[Info] Library for {UserId} created", userId);
            library = await _db.Libraries.FirstOrDefaultAsync(x => x.IdUser == userId);
            return;
        }
        
        
        LibrariesTrack? lt = await _db.LibrariesTracks.FirstOrDefaultAsync(x => 
            x.IdLibrary == library.Id && 
            x.IdTrack == trackId);

        if (lt is null)
        {
            _logger.LogWarning("[Error] Track {TrackId} in Library {LibraryId} doesn't exist", trackId, library.Id);
            throw new KeyNotFoundException("Track in Library doesn't exist");
        }
        _db.LibrariesTracks.Remove(lt);
        await _db.SaveChangesAsync();
        _logger.LogInformation("[Info] Library for {UserId} library was removed", userId);
    }

    public async Task<List<TrackInfoDto>> GetPersonalizedRecommendationsAsync(
        PageResonanceDto recommendationsDto, 
        string? userId = null)
    {
        _logger.LogDebug("[Debug] Building personalized recommendations. Mood={Mood}, Pace={Pace}, UserId={UserId}", 
            recommendationsDto.MoodName ?? "*", recommendationsDto.PaceName ?? "*", userId ?? "anonymous");
        
        // Config validation
        if (!string.IsNullOrWhiteSpace(recommendationsDto.MoodName) && !await _db.Moods.AnyAsync(m => m.MoodName == recommendationsDto.MoodName))
        {
            _logger.LogWarning("[Warn] Invalid mood filter. Mood={Mood}", recommendationsDto.MoodName);
            throw new ArgumentException($"Invalid mood: {recommendationsDto.MoodName}");
        }
        
        if (!string.IsNullOrWhiteSpace(recommendationsDto.PaceName) && !await _db.Paces.AnyAsync(p => p.PaceName == recommendationsDto.PaceName))
        {
            _logger.LogWarning("[Warn] Invalid pace filter. Pace={Pace}", recommendationsDto.PaceName);
            throw new ArgumentException($"Invalid pace: {recommendationsDto.PaceName}");
        }

        // Dynamic query
        var query = _db.PlaylistsTracks
            .AsNoTracking()
            .AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(recommendationsDto.MoodName))
            query = query.Where(pt => pt.IdTrackNavigation.IdMoodNavigation!.MoodName == recommendationsDto.MoodName);
        
        if (!string.IsNullOrWhiteSpace(recommendationsDto.PaceName))
            query = query.Where(pt => pt.IdTrackNavigation.IdPaceNavigation!.PaceName == recommendationsDto.PaceName);
        
        if (!string.IsNullOrWhiteSpace(recommendationsDto.Country))
            query = query.Where(pt => pt.IdTrackNavigation.Country == recommendationsDto.Country);

        var tracks = await query
            .Select(pt => new TrackInfoDto
            {
                Id = pt.IdTrackNavigation.Id,
                Name = pt.IdTrackNavigation.Name,
                CoverUrl = pt.IdTrackNavigation.CoverFileName,
                DurationSec = pt.IdTrackNavigation.DurationSec,
                UserList = _mapper.Map<List<UserInfoDto>>(pt.IdTrackNavigation.TracksAuthors.ToList())
            })
            .Distinct()
            .Take(50)
            .ToListAsync();
        
        _logger.LogInformation("[Info] Personalized recommendations generated. Count={Count}, UserId={UserId}", 
            tracks.Count, userId ?? "anonymous");
        
        return tracks;
    }

    public async Task<PageMyRecordDto> GetRecommendationConfigAsync()
    {
        _logger.LogDebug("[Debug] Fetching recommendation configuration");
        
        var dto = new PageMyRecordDto
        {
            AvailablePaces = await _db.Paces.AsNoTracking().Select(x => x.PaceName).ToListAsync(),
            AvailableMoods = await _db.Moods.AsNoTracking().Select(x => x.MoodName).ToListAsync()
        };
        
        _logger.LogInformation("[Info] Recommendation config loaded. Moods={MoodCount}, Paces={PaceCount}", 
            dto.AvailableMoods.Count, dto.AvailablePaces.Count);
        return dto;
    }

    public async Task<bool> IsLikedAsync(long userId, long trackId)
    {
        _logger.LogDebug("[Debug] Check if track is liked. UserId={UserId}, TrackId={TrackId}", userId, trackId);
        
        var exists = await _db.LibrariesTracks
            .AsNoTracking()
            .AnyAsync(x => x.IdTrack == trackId && x.IdLibraryNavigation.IdUser == userId);
        
        _logger.LogDebug("[Debug] Like status: {IsLiked}. UserId={UserId}, TrackId={TrackId}", 
            exists, userId, trackId);
        return exists;
    }
}