using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Entities;

public partial class AuthEmployee
{
    public int Id { get; set; }

    public int IdEmployee { get; set; }

    public short IdProvider { get; set; }

    public long ExternalId { get; set; }

    public virtual Employee IdEmployeeNavigation { get; set; } = null!;

    public virtual AuthProvider IdProviderNavigation { get; set; } = null!;
}
