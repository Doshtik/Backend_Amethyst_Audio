using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Models.Entities;

public partial class UsersSub
{
    public long Id { get; set; }

    public long IdUser { get; set; }

    public long IdSubscriber { get; set; }

    public DateTime SubscribedAt { get; set; }

    public virtual User IdSubscriberNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
