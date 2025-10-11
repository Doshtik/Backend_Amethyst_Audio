using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models;

public partial class UsersHistory
{
    public long Id { get; set; }

    public long IdUser { get; set; }

    public long IdTrack { get; set; }

    public int TotalListeningSec { get; set; }

    public DateTime ListeningAt { get; set; }

    public virtual Track IdTrackNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
