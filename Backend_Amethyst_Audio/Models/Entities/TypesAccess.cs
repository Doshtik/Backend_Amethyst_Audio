using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models.Entities;

public partial class TypesAccess
{
    public short Id { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
}
