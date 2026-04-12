using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Entities;

public partial class Track
{
    public long Id { get; set; }

    public short? IdPace { get; set; }

    public short? IdMood { get; set; }

    public string Name { get; set; } = null!;

    public string? Country { get; set; }

    public bool? IsTextless { get; set; }

    public bool IsExplicit { get; set; }

    public string CoverFileName { get; set; } = null!;

    public string TrackFileName { get; set; } = null!;

    public int DurationSec { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<AlbumsTrack> AlbumsTracks { get; set; } = new List<AlbumsTrack>();

    public virtual Mood? IdMoodNavigation { get; set; }

    public virtual Pace? IdPaceNavigation { get; set; }

    public virtual ICollection<LibrariesTrack> LibrariesTracks { get; set; } = new List<LibrariesTrack>();

    public virtual ICollection<PlaylistsTrack> PlaylistsTracks { get; set; } = new List<PlaylistsTrack>();

    public virtual ICollection<TracksAuthor> TracksAuthors { get; set; } = new List<TracksAuthor>();

    public virtual ICollection<TracksGenre> TracksGenres { get; set; } = new List<TracksGenre>();

    public virtual ICollection<UsersHistory> UsersHistories { get; set; } = new List<UsersHistory>();
}
