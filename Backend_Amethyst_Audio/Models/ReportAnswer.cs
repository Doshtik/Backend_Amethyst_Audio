using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models;

public partial class ReportAnswer
{
    public long Id { get; set; }

    public int IdEmployee { get; set; }

    public long IdReport { get; set; }

    public string Message { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Employee IdEmployeeNavigation { get; set; } = null!;

    public virtual Report IdReportNavigation { get; set; } = null!;
}
