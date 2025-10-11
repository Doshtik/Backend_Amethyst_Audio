using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models;

public partial class OauthUser
{
    public long Id { get; set; }

    public long IdUser { get; set; }

    public short IdProvider { get; set; }

    public string Email { get; set; } = null!;

    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? TokenExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual OauthProvider IdProviderNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
