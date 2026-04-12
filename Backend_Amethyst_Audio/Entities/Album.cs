using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Entities;

public partial class Album
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string CoverFileName { get; set; } = null!;

    public DateOnly CreatedAt { get; set; }

    public DateOnly UpdatedAt { get; set; }

    public virtual ICollection<AlbumsAuthor> AlbumsAuthors { get; set; } = new List<AlbumsAuthor>();

    public virtual ICollection<AlbumsTrack> AlbumsTracks { get; set; } = new List<AlbumsTrack>();

    public virtual ICollection<SavedAlbum> SavedAlbums { get; set; } = new List<SavedAlbum>();
}
