using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Backend_Amethyst_Audio.Models.Entities;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace Backend_Amethyst_Audio.Services.Implementations;

public class TokenService : ITokenService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<TokenService> _logger;
    private const int TOKEN_LIFETIME_DEYS = 7;

    public TokenService(IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<TokenService> logger)
    {
        _httpClientFactory = httpClientFactory;
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
            expires: DateTime.UtcNow.AddDays(TOKEN_LIFETIME_DEYS), // token lifetime
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public async Task<(long ExternalId, string? Email, string? DisplayName)> ValidateExternalTokenAsync(string provider, string token)
    {
        // Choose unified userinfo-endpoint for both
        var userInfoUrl = provider.ToLowerInvariant() switch
        {
            "google" => "https://www.googleapis.com/oauth2/v3/userinfo",
            "yandex" => "https://login.yandex.ru/info?format=json",
            _ => throw new NotSupportedException($"Provider '{provider}' is not supported.")
        };

        // Single HTTP-request
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync(userInfoUrl);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("[Warn] External token validation failed. Provider={Provider}, Status={Status}", provider, response.StatusCode);
            throw new UnauthorizedAccessException($"Invalid or expired {provider} token.");
        }

        // Unified parsing JSON
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = doc.RootElement;

        // Field mapping
        var externalId = provider.ToLowerInvariant() switch
        {
            "google" => root.GetProperty("sub").GetString(),
            "yandex" => root.GetProperty("id").GetString(),
            _ => throw new NotSupportedException()
        };

        var email = provider.ToLowerInvariant() switch
        {
            "google" => root.TryGetProperty("email", out var eG) ? eG.GetString() : null,
            "yandex" => root.TryGetProperty("default_email", out var eY) ? eY.GetString() : null,
            _ => null
        };

        var displayName = provider.ToLowerInvariant() switch
        {
            "google" => root.TryGetProperty("name", out var nG) ? nG.GetString() : null,
            "yandex" => root.TryGetProperty("display_name", out var nY) ? nY.GetString() 
                             : root.TryGetProperty("login", out var lY) ? lY.GetString() : null,
            _ => null
        };

        if (string.IsNullOrWhiteSpace(externalId))
            throw new UnauthorizedAccessException("Provider did not return a valid user ID.");

        return (long.Parse(externalId!), email, displayName);
    }
}