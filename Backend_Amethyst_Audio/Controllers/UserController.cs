using Backend_Amethyst_Audio.DTO;
using Microsoft.AspNetCore.Mvc;
using Backend_Amethyst_Audio.Models;
using Backend_Amethyst_Audio.Services.Abstractions;

namespace Backend_Amethyst_Audio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) => _userService = userService;
        
        [HttpGet]
        public async Task<ActionResult> GetAllUsers()
        {
            return Ok("Список пользователей");
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserReadDTO>> RegisterUser([FromBody] UserCreateDTO dto)
        {
            var result = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(RegisterUser), new { id = result.Id }, result);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult> GetUserById(int userId)
        {
            return Ok($"Пользователь: {userId}");
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser([FromBody] UserCreateDTO user)
        {
            return Ok("Данные о пользователе изменены");
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> DestroyUser(int userId)
        {
            return Ok($"Пользователь {userId} удалён");
        }
    }
}
