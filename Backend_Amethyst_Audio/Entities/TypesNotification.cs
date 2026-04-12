using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Entities;

public partial class TypesNotification
{
    public short Id { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
