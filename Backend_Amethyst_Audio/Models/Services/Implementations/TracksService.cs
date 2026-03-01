using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.DTO.Pages;
using Backend_Amethyst_Audio.Models.Data;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_Amethyst_Audio.Services.Implementations;

public class TracksService(AppDbContext db) : ITrackService
{
    private IMediaSevice _mediaSevice;
    
    public Task<TrackInfoDto> GetByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<List<TrackInfoDto>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task CreateAsync(CreateTrackDto dto)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(ChangeTrackInfoDto dto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<List<TrackInfoDto>> GetListByGenreAsync(string genre)
    {
        throw new NotImplementedException();
    }

    public Task<List<TrackInfoDto>> GetListBySearchAsync(string search)
    {
        throw new NotImplementedException();
    }

    public Task<List<TrackInfoDto>> GetListByUserIdAsync(long userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<TrackInfoDto>> GetListOfNewestAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<List<TrackInfoDto>> GetListOfLikedAsync(long likedPlaylistId)
    {
        var likedTracks = await db.PlaylistsTracks
            .Where(pt => pt.IdPlaylist == likedPlaylistId)
            .Select(pt => new TrackInfoDto
            {
                Id = pt.IdTrackNavigation.Id,
                Name = pt.IdTrackNavigation.Name,
                CoverUrl = pt.IdTrackNavigation.CoverFileName,
                DurationSec = pt.IdTrackNavigation.DurationSec,
                // Собираем авторов прямо здесь
                UserList = pt.IdTrackNavigation
                    .TracksAuthors
                    .Select(ta => new UserInfoDto
                    {
                        Id = ta.IdAuthorNavigation.Id,
                        Lastname = ta.IdAuthorNavigation.Lastname,
                        Firstname = ta.IdAuthorNavigation.Firstname,
                        Nickname = ta.IdAuthorNavigation.Nickname,
                        Email = ta.IdAuthorNavigation.Email,
                        AvatarUrl = ta.IdAuthorNavigation.AvatarFileName,
                        HeaderUrl = ta.IdAuthorNavigation.HeaderFileName,
                        IsVerified = ta.IdAuthorNavigation.IsVerified,
                    })
                    .ToList()
            })
            .ToListAsync();
        return likedTracks;
    }

    public Task<List<TrackInfoDto>> GetListOfPersonalizedAsync(PageMyRecordPersonalizedDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<PageMyRecordDto> PageMyRecordAsync()
    {
        PageMyRecordDto dto = new PageMyRecordDto
        {
            AvailablePaces = db.Paces.Select(x => x.PaceName).ToList(),
            AvailableMoods = db.Moods.Select(x => x.MoodName).ToList()
        };
        return Task.FromResult(dto);
    }

    public Task<bool> IsLikedAsync(long idUser)
    {
        throw new NotImplementedException();
    }

    public Task AddInPlaylistAsync(long idUser, long idPlaylist)
    {
        throw new NotImplementedException();
    }

    public Task RemoveFromPlaylistAsync(long idUser, long idPlaylist)
    {
        throw new NotImplementedException();
    }

    public IActionResult GetImageByName(string name)
    {
        throw new NotImplementedException();
    }
}