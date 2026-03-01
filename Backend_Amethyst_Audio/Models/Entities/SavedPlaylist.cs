using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models.Entities;

public partial class SavedPlaylist
{
    public long Id { get; set; }

    public long IdPlaylist { get; set; }

    public long IdUser { get; set; }

    public virtual Playlist IdPlaylistNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
