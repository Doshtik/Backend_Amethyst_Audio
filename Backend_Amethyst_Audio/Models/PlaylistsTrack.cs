using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models;

public partial class PlaylistsTrack
{
    public long Id { get; set; }

    public long IdPlaylist { get; set; }

    public long IdTrack { get; set; }

    public virtual Playlist IdPlaylistNavigation { get; set; } = null!;

    public virtual Track IdTrackNavigation { get; set; } = null!;
}
