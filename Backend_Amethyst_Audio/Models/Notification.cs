using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models;

public partial class Notification
{
    public long Id { get; set; }

    public long IdUser { get; set; }

    public string Subject { get; set; } = null!;

    public string Body { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;
}
