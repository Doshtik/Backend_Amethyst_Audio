using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models;

public partial class TracksAuthor
{
    public long Id { get; set; }

    public long IdTrack { get; set; }

    public long IdAuthor { get; set; }

    public virtual User IdAuthorNavigation { get; set; } = null!;

    public virtual Track IdTrackNavigation { get; set; } = null!;
}
