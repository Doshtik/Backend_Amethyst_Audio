using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Services.Abstractions;
using Backend_Amethyst_Audio.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Amethyst_Audio.Controllers;

[ApiController]
[Route("api/{controller}")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("signin")]
    public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserDto dto)
    {
        try
        {
            _logger.LogInformation("[Info] User registration attempt. Email={Email}", dto.Email);
            
            UserInfoDto result = await _userService.CreateAsync(dto);
            
            _logger.LogInformation("[Info] User registered successfully. UserId={UserId}, Email={Email}", 
                result.Id, dto.Email);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (BadHttpRequestException e)
        {
            _logger.LogWarning("[Warn] Validation failed during registration. Email={Email}, Reason={Reason}", 
                dto.Email, e.Message);
            return BadRequest(new { error = e.Message });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Critical error during user registration. Email={Email}", dto.Email);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
    
    [HttpGet("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDto dto)
    {
        try
        {
            _logger.LogInformation("[Info] Login attempt. Email={Email}", dto.Email);
            
            UserInfoDto user = await _userService.LoginAsync(dto);
            
            _logger.LogInformation("[Info] Login successful. UserId={UserId}, Email={Email}", 
                user.Id, dto.Email);
            return Ok(user);
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogWarning("[Warn] User not found during login. Email={Email}", dto.Email);
            return NotFound(new { error = e.Message });
        }
        catch (UnauthorizedAccessException e)
        {
            _logger.LogWarning("[Warn] Invalid password for user. Email={Email}", dto.Email);
            return Unauthorized(new { error = e.Message });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Login failed. Email={Email}", dto.Email);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpPost("external-login")]
    public async Task<IActionResult> ExternalLoginAsync([FromBody] ExternalLoginDto dto)
    {
        try
        {
            _logger.LogInformation("[Info] External login attempt. Provider={Provider}", dto.Provider);
        
            var result = await _userService.ExternalLoginAsync(dto);
        
            _logger.LogInformation("[Info] External login successful. UserId={UserId}", result.Id);
            return Ok(result);
        }
        catch (NotSupportedException ex)
        {
            _logger.LogWarning("[Warn] Unsupported provider. Provider={Provider}", dto.Provider);
            return BadRequest(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("[Warn] Invalid external token. Provider={Provider}", dto.Provider);
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Error] External login failed. Provider={Provider}", dto.Provider);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /*
     * На данный момент данный маршрут не нужен, потому что у меня примитивный JWT
     */
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync(/*[FromBody] RefreshRequestDto dto*/)
    {
        try
        {
            _logger.LogDebug("[Debug] Token refresh request received");
            // TODO: Implement token refresh logic
            return Ok(new { token = "new_token", refreshToken = "new_refresh_token" });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Token refresh failed");
            return StatusCode(500, new { error = "Failed to refresh token" });
        }
    }

    /*
     * На данный момент данный маршрут не нужен, потому что у меня примитивный JWT
     */
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation("[Info] Logout request. UserId={UserId}", userId);
        
        try
        {
            await _userService.LogoutAsync(long.Parse(userId));
            _logger.LogInformation("[Info] User logged out successfully. UserId={UserId}", userId);
            return Ok(new { message = "Logged out" });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Logout failed. UserId={UserId}", userId);
            return StatusCode(500, new { error = "Logout failed" });
        }
    }
}