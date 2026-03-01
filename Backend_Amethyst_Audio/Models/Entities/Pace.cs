using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models.Entities;

public partial class Pace
{
    public short Id { get; set; }

    public string PaceName { get; set; } = null!;

    public virtual ICollection<Track> Tracks { get; set; } = new List<Track>();
}
