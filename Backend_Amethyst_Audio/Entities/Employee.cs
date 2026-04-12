using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Entities;

public partial class Employee
{
    public int Id { get; set; }

    public short IdRole { get; set; }

    public string? Lastname { get; set; }

    public string? Firstname { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime LastVisit { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<AuthEmployee> AuthEmployees { get; set; } = new List<AuthEmployee>();

    public virtual Role IdRoleNavigation { get; set; } = null!;

    public virtual ICollection<ReportAnswer> ReportAnswers { get; set; } = new List<ReportAnswer>();
}
