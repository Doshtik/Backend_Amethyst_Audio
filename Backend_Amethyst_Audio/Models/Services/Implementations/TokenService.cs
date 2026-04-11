using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend_Amethyst_Audio.Models.Entities;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace Backend_Amethyst_Audio.Services.Implementations;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly ILogger<TokenService> _logger;
    private const int TOKEN_LIFETIME_DEYS = 7;

    public TokenService(IConfiguration config, ILogger<TokenService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Nickname),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(TOKEN_LIFETIME_DEYS), // Срок жизни токена
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}