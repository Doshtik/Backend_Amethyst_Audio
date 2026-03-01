using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models.Entities;

public partial class Mood
{
    public short Id { get; set; }

    public string MoodName { get; set; } = null!;

    public virtual ICollection<Track> Tracks { get; set; } = new List<Track>();
}
