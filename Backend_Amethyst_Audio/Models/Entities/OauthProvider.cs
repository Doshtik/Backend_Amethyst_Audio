using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models.Entities;

public partial class OauthProvider
{
    public short Id { get; set; }

    public string ProviderName { get; set; } = null!;

    public virtual ICollection<OauthEmployee> OauthEmployees { get; set; } = new List<OauthEmployee>();

    public virtual ICollection<OauthUser> OauthUsers { get; set; } = new List<OauthUser>();
}
