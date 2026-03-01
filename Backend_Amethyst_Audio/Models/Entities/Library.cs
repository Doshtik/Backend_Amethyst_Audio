using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models.Entities;

public partial class Library
{
    public long Id { get; set; }

    public long IdUser { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<LibrariesTrack> LibrariesTracks { get; set; } = new List<LibrariesTrack>();
}
