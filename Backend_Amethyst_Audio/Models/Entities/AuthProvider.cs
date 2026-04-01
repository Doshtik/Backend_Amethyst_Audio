using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models.Entities;

public partial class AuthProvider
{
    public short Id { get; set; }

    public string ProviderName { get; set; } = null!;

    public virtual ICollection<AuthEmployee> OauthEmployees { get; set; } = new List<AuthEmployee>();

    public virtual ICollection<AuthUser> OauthUsers { get; set; } = new List<AuthUser>();
}
