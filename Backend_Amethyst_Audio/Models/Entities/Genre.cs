using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models.Entities;

public partial class Genre
{
    public short Id { get; set; }

    public string GenreName { get; set; } = null!;

    public virtual ICollection<TracksGenre> TracksGenres { get; set; } = new List<TracksGenre>();
}
