using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models.Entities;

public partial class Report
{
    public long Id { get; set; }

    public long IdUser { get; set; }

    public short IdType { get; set; }

    public short IdReason { get; set; }

    public string? Discription { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Reason IdReasonNavigation { get; set; } = null!;

    public virtual TypesReport IdTypeNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<ReportAnswer> ReportAnswers { get; set; } = new List<ReportAnswer>();
}
