using System;
using System.Collections.Generic;

namespace MyLittleLife.DataApi.Models;

public partial class NeedType
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal DecayPerSecond { get; set; }

    public virtual ICollection<ActionType> ActionTypes { get; set; } = new List<ActionType>();

    public virtual ICollection<CharacterNeed> CharacterNeeds { get; set; } = new List<CharacterNeed>();

    public virtual ICollection<NeedHistory> NeedHistories { get; set; } = new List<NeedHistory>();
}
