using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Entities;

public partial class AlbumsAuthor
{
    public long Id { get; set; }

    public long IdAlbum { get; set; }

    public long IdAuthor { get; set; }

    public virtual Album IdAlbumNavigation { get; set; } = null!;

    public virtual User IdAuthorNavigation { get; set; } = null!;
}
