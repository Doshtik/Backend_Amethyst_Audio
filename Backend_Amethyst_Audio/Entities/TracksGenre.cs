using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Entities;

public partial class TracksGenre
{
    public long Id { get; set; }

    public long IdTrack { get; set; }

    public short IdGenre { get; set; }

    public virtual Genre IdGenreNavigation { get; set; } = null!;

    public virtual Track IdTrackNavigation { get; set; } = null!;
}
