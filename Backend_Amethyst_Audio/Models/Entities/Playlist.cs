using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models.Entities;

public partial class Playlist
{
    public long Id { get; set; }

    public long IdUser { get; set; }

    public bool IsPublic { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? CoverFileName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<PlaylistsTrack> PlaylistsTracks { get; set; } = new List<PlaylistsTrack>();

    public virtual ICollection<SavedPlaylist> SavedPlaylists { get; set; } = new List<SavedPlaylist>();
}
