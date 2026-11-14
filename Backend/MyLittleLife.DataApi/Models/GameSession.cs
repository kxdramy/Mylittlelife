using System;
using System.Collections.Generic;

namespace MyLittleLife.DataApi.Models;

public partial class GameSession
{
    public long Id { get; set; }

    public long SaveSlotId { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public virtual ICollection<ActionHistory> ActionHistories { get; set; } = new List<ActionHistory>();

    public virtual ICollection<NeedHistory> NeedHistories { get; set; } = new List<NeedHistory>();

    public virtual SaveSlot SaveSlot { get; set; } = null!;
}
