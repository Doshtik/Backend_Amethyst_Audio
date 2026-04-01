using Backend_Amethyst_Audio.Models.Entities;

namespace Backend_Amethyst_Audio.Services.Abstractions;

public interface ITokenService
{
    string GenerateJwtToken(User user);
}