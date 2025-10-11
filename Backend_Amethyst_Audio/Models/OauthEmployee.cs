using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models;

public partial class OauthEmployee
{
    public int Id { get; set; }

    public int IdEmployee { get; set; }

    public short IdProvider { get; set; }

    public string Email { get; set; } = null!;

    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? TokenExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Employee IdEmployeeNavigation { get; set; } = null!;

    public virtual OauthProvider IdProviderNavigation { get; set; } = null!;
}
