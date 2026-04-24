using System.Security.Claims;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Amethyst_Audio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITrackService _trackService;
    private readonly IAlbumService _albumService;
    private readonly IPlaylistService _playlistService;

    public SearchController(IUserService userService, ITrackService trackService, IAlbumService albumService, IPlaylistService playlistService)
    {
        _userService = userService;
        _trackService = trackService;
        _albumService = albumService;
        _playlistService = playlistService;
    }

    [HttpGet("genres")]
    public async Task<IActionResult> GetGenres()
    {
        GenreInfoDto dto = await _trackService.GetListGenresAsync();
        return Ok(dto);
    }

    [HttpGet("genres/{genreName}")]
    public async Task<IActionResult> GetListByGenre(string genreName)
    {
        List<TrackInfoDto> tracks = await _trackService.GetListByGenreAsync(genreName);
        return Ok(tracks);
    }

    [HttpGet("{searchLine}")]
    [Authorize]
    public async Task<IActionResult> GetBySearch(string searchLine)
    {
        long userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        
        List<UserInfoDto> users = await _userService.GetListByNicknameAsync(searchLine);
        List<TrackInfoDto> tracks = await _trackService.GetListByTrackNameAsync(searchLine);
        List<AlbumInfoDto> albums = await _albumService.GetListByAlbumNameAsync(searchLine);
        List<PlaylistInfoDto> playlists = await _playlistService.GetListByPlaylistNameAsync(userId, searchLine);

        SearchInfoDto searchInfoDto = new SearchInfoDto()
        {
            Users = users,
            Tracks = tracks,
            Albums = albums,
            Playlists = playlists
        };
        
        return Ok(searchInfoDto);
    }
}