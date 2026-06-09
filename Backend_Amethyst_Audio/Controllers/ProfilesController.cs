using System.Security.Claims;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Amethyst_Audio.Controllers;

[ApiController]
[Route("api/{controller}")]
public class ProfilesController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITrackService _trackService;
    private readonly IPlaylistService _playlistService;
    private readonly IAlbumService _albumService;
    private readonly ILogger<ProfilesController> _logger;
    
    public ProfilesController(IUserService userService, ITrackService trackService, IPlaylistService playlistService, 
        IAlbumService albumService, ILogger<ProfilesController> logger) 
    {
        _userService = userService;
        _trackService = trackService;
        _playlistService = playlistService;
        _albumService = albumService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetByIdAsync(long id)
    {
        try
        {
            _logger.LogDebug("[Debug] Get user by Id request. TargetUserId={UserId}", id);
            
            UserInfoDto userDto = await _userService.GetByIdAsync(id);
            
            _logger.LogInformation("[Info] User retrieved successfully. UserId={UserId}", id);
            return Ok(userDto);
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogWarning("[Warn] User not found. UserId={UserId}", id);
            return NotFound(new { error = e.Message });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to get user. UserId={UserId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("[Info] Get all users request. RequestedBy={UserId}", 
                User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous");
            
            List<UserInfoDto> listDto = await _userService.GetAllAsync();
            
            _logger.LogInformation("[Info] Retrieved {Count} users", listDto.Count);
            return Ok(listDto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to retrieve users list");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet("sub-count/{userId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAmountOfSubsAsync(long userId)
    {
        _logger.LogDebug("[Debug] Request to get amount of listeners by user Id: {Id}", userId);
        try
        { 
            int amount = await _userService.GetSubscriberAmountAsync(userId);
            _logger.LogInformation("[Info] Successfully retrieved {Amount} tracks", amount);
            return Ok(amount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Error] Unexpected error retrieving all user tracks");
            return Problem(statusCode: 500, title: "Internal Server Error", detail: "An error occurred while processing your request.");
        }
    }
    
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateAsync(long id, [FromForm] ChangeUserInfoDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        // Check: only user can edit his own profile
        if (currentUserId != id.ToString())
        {
            _logger.LogWarning("[Warn] Unauthorized update attempt. CurrentUserId={CurrentId}, TargetUserId={TargetId}", 
                currentUserId, id);
            return Forbid();
        }
        
        try
        {
            _logger.LogInformation("[Info] Update user request. UserId={UserId}, UpdatedFields={Fields}", 
                id, string.Join(",", dto.GetType().GetProperties()
                    .Where(p => p.GetValue(dto) != null)
                    .Select(p => p.Name)));
            
            await _userService.UpdateAsync(id, dto);
            
            _logger.LogInformation("[Info] User data updated successfully. UserId={UserId}", id);
            return Ok(new { message = "User data updated successfully" });
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogWarning("[Warn] User not found for update. UserId={UserId}", id);
            return NotFound(new { error = e.Message });
        }
        catch (ArgumentException e)
        {
            _logger.LogWarning("[Warn] Validation failed during update. UserId={UserId}, Reason={Reason}", 
                id, e.Message);
            return BadRequest(new { error = e.Message });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to update user. UserId={UserId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpPatch("{id}")]
    [Authorize]
    public async Task<IActionResult> ChangeUserPasswordAsync(long id, [FromBody] ChangeUserPasswordDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (currentUserId != id.ToString())
        {
            _logger.LogWarning("[Warn] Unauthorized password change attempt. CurrentUserId={CurrentId}, TargetUserId={TargetId}", 
                currentUserId, id);
            return Forbid();
        }
        
        try
        {
            _logger.LogInformation("[Info] Password change request. UserId={UserId}", id);
            
            await _userService.ChangePasswordAsync(id, dto);
            
            _logger.LogInformation("[Info] Password changed successfully. UserId={UserId}", id);
            return Ok(new { message = "Password changed successfully" });
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogWarning("[Warn] User not found for password change. UserId={UserId}", id);
            return NotFound(new { error = e.Message });
        }
        catch (UnauthorizedAccessException e)
        {
            _logger.LogWarning("[Warn] Invalid current password. UserId={UserId}", id);
            return Unauthorized(new { error = e.Message });
        }
        catch (ArgumentException e)
        {
            _logger.LogWarning("[Warn] Password validation failed. UserId={UserId}, Reason={Reason}", id, e.Message);
            return BadRequest(new { error = e.Message });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to change password. UserId={UserId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (currentUserId != id.ToString())
        {
            _logger.LogWarning("[Warn] Unauthorized delete attempt. CurrentUserId={CurrentId}, TargetUserId={TargetId}", 
                currentUserId, id);
            return Forbid();
        }
        
        try
        {
            _logger.LogInformation("[Info] Delete user request. UserId={UserId}, RequestedBy={RequestId}", 
                id, currentUserId);
            
            await _userService.DeleteAsync(id);
            
            _logger.LogInformation("[Info] User deleted successfully. UserId={UserId}", id);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogWarning("[Warn] User not found for deletion. UserId={UserId}", id);
            return NotFound(new { error = e.Message });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to delete user. UserId={UserId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
    
    [HttpGet("history")]
    [Authorize]
    public async Task<IActionResult> GetUserHistoryAsync()
    {
        long userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        List<UserHistoryDto> dto = await _userService.GetUserHistoryAsync(userId);
        return Ok(dto);
    }

    [HttpPost("history/{trackId}")]
    [Authorize]
    public async Task<IActionResult> AddToHistoryAsync(long trackId)
    {
        long userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        try
        {
            await _userService.AddToHistoryAsync(userId, trackId);
            return Created();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { error = e.Message });
        }
    }

    [HttpPut("history/{trackId}")]
    [Authorize]
    public async Task<IActionResult> UpdateListeningTimeAsync(long trackId, [FromQuery] int seconds)
    {
        long userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        try
        {
            await _userService.UpdateListeningTimeAsync(userId, trackId, seconds);
            return Ok();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { error = e.Message });
        }
        
    }

    //TODO: GetUserLibraryAsync (Вроде бы сделал)
    [HttpGet("library/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetUserLibraryAsync(long userId)
    {
        var list = await _trackService.GetUserLibraryAsync(userId);
        return Ok(list);
    }
    
    //TODO: AddTrackToLibraryAsync (Вроде бы сделал)
    [HttpPost("library/track/{trackId}")]
    [Authorize]
    public async Task<IActionResult> AddTrackToLibraryAsync(long trackId)
    {
        try
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _trackService.AddTrackToLibraryAsync(trackId, userId);
            return Ok("Track added successfully");
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { error = e.Message });
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { error = e.Message });
        }
        
    }
    
    //TODO: RemoveTrackToLibraryAsync (Вроде бы сделал)
    [HttpDelete("library/track/{trackId}")]
    [Authorize]
    public async Task<IActionResult> RemoveTrackToLibraryAsync(long trackId)
    {
        try
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _trackService.RemoveTrackToLibraryAsync(trackId, userId);
            return Ok("Track removed successfully");
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { error = e.Message });
        }
    }
    
    //TODO: GetUserSavedPlaylistsAsync (Вроде бы сделал)
    [HttpGet("library/playlists/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetUserSavedPlaylistsAsync(long userId)
    {
        var list = await _playlistService.GetListSavedAsync(userId);
        return Ok(list);
    }
    
    //TODO: GetUserSavedAlbumsAsync (Вроде бы сделал)
    [HttpGet("library/albums/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetUserSavedAlbumsAsync(long userId)
    {
        var list = await _albumService.GetListSavedAsync(userId);
        return Ok(list);
    }
    
    [HttpGet("subscription/{targetUserId}")]
    [Authorize]
    public async Task<IActionResult> IsUserFollowedAsync(long targetUserId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var currentUserId))
        {
            _logger.LogWarning("[Warn] Follow attempt with invalid or missing user claim");
            return Unauthorized(new { error = "Authentication required" });
        }
        
        try
        {
            bool result = await _userService.IsUserFollowedAsync(currentUserId, targetUserId);
            
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to check following. SubscriberId={currentUserId}, TargetUserId={targetUserId}", 
                currentUserId, targetUserId);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
    
    [HttpPost("subscription")]
    [Authorize]
    public async Task<IActionResult> FollowUserAsync([FromBody] FollowUserDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var currentUserId))
        {
            _logger.LogWarning("[Warn] Follow attempt with invalid or missing user claim");
            return Unauthorized(new { error = "Authentication required" });
        }

        // Security: override IdSubscriber with the token data to prevent the client from spoofing the subscriber
        dto.IdSubscriber = currentUserId;
        
        try
        {
            _logger.LogInformation("[Info] Follow request. SubscriberId={SubscriberId} → TargetUserId={TargetUserId}", 
                dto.IdSubscriber, dto.IdTargetUser);
            
            await _userService.FollowAsync(dto);
            
            _logger.LogInformation("[Info] Follow successful. SubscriberId={SubscriberId} → TargetUserId={TargetUserId}", 
                dto.IdSubscriber, dto.IdTargetUser);
            return Ok(new { message = "Successfully followed user" });
        }
        catch (ArgumentException e)
        {
            _logger.LogWarning("[Warn] Validation failed for follow request. SubscriberId={SubscriberId}, TargetUserId={TargetUserId}, Reason={Reason}", 
                dto.IdSubscriber, dto.IdTargetUser, e.Message);
            return BadRequest(new { error = e.Message });
        }
        catch (InvalidOperationException e)
        {
            _logger.LogWarning("[Warn] Follow conflict. SubscriberId={SubscriberId}, TargetUserId={TargetUserId}, Reason={Reason}", 
                dto.IdSubscriber, dto.IdTargetUser, e.Message);
            return Conflict(new { error = e.Message });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to process follow request. SubscriberId={SubscriberId}, TargetUserId={TargetUserId}", 
                dto.IdSubscriber, dto.IdTargetUser);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpDelete("subscription")]
    [Authorize]
    public async Task<IActionResult> UnfollowUserAsync([FromBody] FollowUserDto dto)
    {
        var userIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
    
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var currentUserId))
        {
            _logger.LogWarning("[Warn] Unfollow attempt with invalid or missing user claim");
            return Unauthorized(new { error = "Authentication required" });
        }

        dto.IdSubscriber = currentUserId;
        
        try
        {
            _logger.LogInformation("[Info] Unfollow request. SubscriberId={SubscriberId} → TargetUserId={TargetUserId}", 
                dto.IdSubscriber, dto.IdTargetUser);
            
            await _userService.UnfollowAsync(dto);
            
            _logger.LogInformation("[Info] Unfollow successful. SubscriberId={SubscriberId} → TargetUserId={TargetUserId}", 
                dto.IdSubscriber, dto.IdTargetUser);
            return Ok(new { message = "Successfully unfollowed user" });
        }
        catch (ArgumentException e)
        {
            _logger.LogWarning("[Warn] Validation failed for unfollow request. SubscriberId={SubscriberId}, TargetUserId={TargetUserId}, Reason={Reason}", 
                dto.IdSubscriber, dto.IdTargetUser, e.Message);
            return BadRequest(new { error = e.Message });
        }
        catch (InvalidOperationException e)
        {
            _logger.LogWarning("[Warn] Unfollow conflict. SubscriberId={SubscriberId}, TargetUserId={TargetUserId}, Reason={Reason}", 
                dto.IdSubscriber, dto.IdTargetUser, e.Message);
            return NotFound(new { error = e.Message });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to process unfollow request. SubscriberId={SubscriberId}, TargetUserId={TargetUserId}", 
                dto.IdSubscriber, dto.IdTargetUser);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}