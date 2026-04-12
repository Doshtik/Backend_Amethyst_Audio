using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Entities;

public partial class Reason
{
    public short Id { get; set; }

    public string ReasonName { get; set; } = null!;

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
