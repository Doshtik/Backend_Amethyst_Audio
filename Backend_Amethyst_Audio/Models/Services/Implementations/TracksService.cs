using AutoMapper;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.DTO.Pages;
using Backend_Amethyst_Audio.Models.Data;
using Backend_Amethyst_Audio.Models.Entities;
using Backend_Amethyst_Audio.Profiles;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_Amethyst_Audio.Services.Implementations;

public class TracksService(AppDbContext db, IMapper mapper) : ITrackService
{
    private readonly string _baseUrl;
    private IMediaSevice _mediaSevice;
    
    public async Task<TrackInfoDto> GetByIdAsync(long id)
    {
        Track track = await db.Tracks.FindAsync(id);
        return mapper.Map<TrackInfoDto>(track);
    }

    public async Task<List<TrackInfoDto>> GetAllAsync()
    {
        List<Track> tracks = await db.Tracks.ToListAsync();
        return mapper.Map<List<TrackInfoDto>>(tracks);
    }

    public async Task<TrackInfoDto> CreateAsync(CreateTrackDto dto)
    {
        Track track = mapper.Map<Track>(dto);
        await db.Tracks.AddAsync(track);
        await db.SaveChangesAsync();
        track.Id = track.Id; // TODO: Проверить способ на достоверность
        return mapper.Map<TrackInfoDto>(track);
    }

    public async Task<TrackInfoDto> UpdateAsync(ChangeTrackInfoDto dto)
    {
        Track track = mapper.Map<Track>(dto);
        db.Tracks.Update(track);
        await db.SaveChangesAsync();
        return mapper.Map<TrackInfoDto>(track);
    }

    public async Task DeleteAsync(long id)
    {
        Track track = await db.Tracks.FindAsync(id);
        if  (track == null)
            throw new KeyNotFoundException();
    }

    public async Task<List<TrackInfoDto>> GetListByGenreAsync(string genre)
    {
        List<Track> tracks = await db.TracksGenres
            .Where(x => x.IdGenreNavigation.GenreName == genre)
            .Select(x => x.IdTrackNavigation)
            .ToListAsync();
        return mapper.Map<List<TrackInfoDto>>(tracks);
    }

    public async Task<List<TrackInfoDto>> GetListByTrackNameAsync(string trackName)
    {
        List<Track> tracks = await db.Tracks.Where(x => x.Name == trackName).ToListAsync();
        return mapper.Map<List<TrackInfoDto>>(tracks);
    }

    public async Task<List<TrackInfoDto>> GetListByUserIdAsync(long userId)
    {
        List<Track> tracks = await db.TracksAuthors
            .Where(x => x.IdAuthor == userId)
            .Select(x => x.IdTrackNavigation)
            .ToListAsync();
        return mapper.Map<List<TrackInfoDto>>(tracks);
    }

    public async Task<List<TrackInfoDto>> GetListOfNewestAsync()
    {
        DateTime thresholdDate = DateTime.Today.AddDays(-60);
        List<Track> tracks = await db.Tracks
            .Where(x => x.CreatedAt >= thresholdDate)
            .ToListAsync<Track>();
        return mapper.Map<List<TrackInfoDto>>(tracks);
    }

    public async Task<List<TrackInfoDto>> GetListOfLikedAsync(long likedPlaylistId)
    {
        List<TrackInfoDto> likedTracks = await db.PlaylistsTracks
            .Where(pt => pt.IdPlaylist == likedPlaylistId)
            .Select(pt => new TrackInfoDto
            {
                Id = pt.IdTrackNavigation.Id,
                Name = pt.IdTrackNavigation.Name,
                CoverUrl = pt.IdTrackNavigation.CoverFileName,
                DurationSec = pt.IdTrackNavigation.DurationSec,
                UserList = mapper.Map<List<UserInfoDto>>(pt.IdTrackNavigation.TracksAuthors.ToList())
            })
            .ToListAsync();
        return likedTracks;
    }

    public async Task<List<TrackInfoDto>> GetListOfPersonalizedAsync(PageMyRecordPersonalizedDto dto)
    {
        List<TrackInfoDto> personalized = await db.PlaylistsTracks
            .Where(pt => 
                pt.IdTrackNavigation.IdMoodNavigation.MoodName == dto.MoodName ||
                pt.IdTrackNavigation.IdPaceNavigation.PaceName == dto.PaceName ||
                pt.IdTrackNavigation.Country == dto.Country)
            .Select(pt => new TrackInfoDto
            {
                Id = pt.IdTrackNavigation.Id,
                Name = pt.IdTrackNavigation.Name,
                CoverUrl = pt.IdTrackNavigation.CoverFileName,
                DurationSec = pt.IdTrackNavigation.DurationSec,
                UserList = mapper.Map<List<UserInfoDto>>(pt.IdTrackNavigation.TracksAuthors.ToList())
            })
            .ToListAsync();
        return personalized;
    }

    public async Task<PageMyRecordDto> PageMyRecordAsync()
    {
        PageMyRecordDto dto = new PageMyRecordDto
        {
            AvailablePaces = await db.Paces.Select(x => x.PaceName).ToListAsync(),
            AvailableMoods = await db.Moods.Select(x => x.MoodName).ToListAsync()
        };
        return dto;
    }

    public async Task<bool> IsLikedAsync(long idUser,  long idTrack)
    {
        Track? track = await db.LibrariesTracks
            .Where(x => x.IdTrack == idTrack && x.IdLibraryNavigation.IdUser == idUser)
            .Select(x => x.IdTrackNavigation)
            .FirstOrDefaultAsync();
        if (track == null)
            return false;
        return true;
    }

    public async Task AddInPlaylistAsync(long idTrack, long idPlaylist)
    {
        
        PlaylistsTrack? playlistsTrack = db.PlaylistsTracks
            .FirstOrDefault(x => x.IdTrack == idTrack && x.IdPlaylist == idPlaylist);
        if (playlistsTrack != null)
            throw new Exception("Playlist already exists");

        playlistsTrack = new PlaylistsTrack();
        playlistsTrack.IdTrack = idTrack;
        playlistsTrack.IdPlaylist = idPlaylist;
        await db.PlaylistsTracks.AddAsync(playlistsTrack);
        await db.SaveChangesAsync();
    }

    public async Task RemoveFromPlaylistAsync(long idTrack, long idPlaylist)
    {
        PlaylistsTrack? playlistsTrack = await db.PlaylistsTracks
            .FirstOrDefaultAsync(x => x.IdTrack == idTrack && x.IdPlaylist == idPlaylist);
        if (playlistsTrack == null)
            throw new Exception("Playlist track not found");
        db.PlaylistsTracks.Remove(playlistsTrack);
        await db.SaveChangesAsync();
    }
}