using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models;

public partial class Playlist
{
    public long Id { get; set; }

    public long IdUser { get; set; }

    public short IdAccessType { get; set; }

    public string Name { get; set; } = null!;

    public string? Discription { get; set; }

    public string? CoverName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual TypesAccess IdAccessTypeNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<PlaylistsTrack> PlaylistsTracks { get; set; } = new List<PlaylistsTrack>();

    public virtual ICollection<SavedPlaylist> SavedPlaylists { get; set; } = new List<SavedPlaylist>();
}
