using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models;

public partial class Track
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public bool? Textless { get; set; }

    public bool ExplicitContent { get; set; }

    public string CoverName { get; set; } = null!;

    public string? Country { get; set; }

    public string? TrackPace { get; set; }

    public string? TrackMood { get; set; }

    public string TrackName { get; set; } = null!;

    public int TrackWeight { get; set; }

    public string TrackMimeType { get; set; } = null!;

    public int DurationSec { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<AlbumsTrack> AlbumsTracks { get; set; } = new List<AlbumsTrack>();

    public virtual ICollection<PlaylistsTrack> PlaylistsTracks { get; set; } = new List<PlaylistsTrack>();

    public virtual ICollection<TracksAuthor> TracksAuthors { get; set; } = new List<TracksAuthor>();

    public virtual ICollection<TracksGenre> TracksGenres { get; set; } = new List<TracksGenre>();

    public virtual ICollection<UsersHistory> UsersHistories { get; set; } = new List<UsersHistory>();
}
