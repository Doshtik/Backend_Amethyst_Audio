using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models.Entities;

public partial class Notification
{
    public long Id { get; set; }

    public short IdType { get; set; }

    public long IdUser { get; set; }

    public string Title { get; set; } = null!;

    public string Body { get; set; } = null!;

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual TypesNotification IdTypeNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
