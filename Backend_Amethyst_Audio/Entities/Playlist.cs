using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Entities;

public partial class Playlist
{
    public long Id { get; set; }

    public long IdUser { get; set; }

    public bool IsPublic { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string CoverFileName { get; set; } = null!;

    public DateOnly CreatedAt { get; set; }

    public DateOnly UpdatedAt { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<PlaylistsTrack> PlaylistsTracks { get; set; } = new List<PlaylistsTrack>();

    public virtual ICollection<SavedPlaylist> SavedPlaylists { get; set; } = new List<SavedPlaylist>();
}
