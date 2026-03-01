using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models.Entities;

public partial class LibrariesTrack
{
    public long Id { get; set; }

    public long IdLibrary { get; set; }

    public long IdTrack { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Library IdLibraryNavigation { get; set; } = null!;

    public virtual Track IdTrackNavigation { get; set; } = null!;
}
