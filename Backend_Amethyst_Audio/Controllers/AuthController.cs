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
    private IUserService _userService;
    private IAuthService _authService;
    
    public AuthController(IUserService userService) => _userService = userService;
    
    // Регистрация
    [HttpPost("signin")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        try
        {
            UserInfoDto result = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(CreateUser), new { id = result.Id }, result);
        }
        catch (BadHttpRequestException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
    // Обычный вход
    [HttpGet("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            UserInfoDto user = await _userService.GetLoginAsync(dto);
            return Ok(user);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound();
        }
    }

    // Вход через Google/другие провайдеры
    [HttpGet("login/{provider}")]
    public IActionResult ExternalLogin(string provider, [FromBody] LoginDto dto)
    {
        return Ok("");
    }

    // Обновление пары токенов
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(/*[FromBody] RefreshRequestDto dto*/)
    {
        return Ok("");
    }

    // Выход (аннулирование Refresh токена в БД)
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        return Ok("");
    }
}