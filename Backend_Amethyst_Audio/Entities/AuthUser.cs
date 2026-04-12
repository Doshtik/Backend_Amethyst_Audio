using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Entities;

public partial class AuthUser
{
    public long Id { get; set; }

    public long IdUser { get; set; }

    public short IdProvider { get; set; }

    public long ExternalId { get; set; }

    public virtual AuthProvider IdProviderNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
