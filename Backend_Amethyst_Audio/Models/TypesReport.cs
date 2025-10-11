using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models;

public partial class TypesReport
{
    public short Id { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
