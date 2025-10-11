using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models;

public partial class SavedAlbum
{
    public long Id { get; set; }

    public long IdAlbum { get; set; }

    public long IdUser { get; set; }

    public virtual Album IdAlbumNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
