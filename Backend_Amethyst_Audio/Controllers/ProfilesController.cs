using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Amethyst_Audio.Controllers;

[ApiController]
[Route("api/{controller}")]
public class ProfilesController : ControllerBase
{
    private IUserService _userService;
    
    public ProfilesController(IUserService userService) => _userService = userService;
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(long id, [FromBody] ChangeUserInfoDto dto)
    {
        try
        {
            await _userService.UpdateAsync(id, dto);
            return Ok("Данные о пользователе успешно изменены");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPatch("{id}")]
    public IActionResult ChangeUserPassword(long id, [FromBody] ChangeUserPasswordDto dto)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(long id)
    {
        throw new NotImplementedException();
    }
}