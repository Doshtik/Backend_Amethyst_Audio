using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models;

public partial class Album
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string CoverName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<AlbumsAuthor> AlbumsAuthors { get; set; } = new List<AlbumsAuthor>();

    public virtual ICollection<AlbumsTrack> AlbumsTracks { get; set; } = new List<AlbumsTrack>();

    public virtual ICollection<SavedAlbum> SavedAlbums { get; set; } = new List<SavedAlbum>();
}
