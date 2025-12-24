using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models;

public partial class AlbumsTrack
{
    public long Id { get; set; }

    public long IdAlbum { get; set; }

    public long IdTrack { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Album IdAlbumNavigation { get; set; } = null!;

    public virtual Track IdTrackNavigation { get; set; } = null!;
}
