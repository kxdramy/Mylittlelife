using System;
using System.Collections.Generic;

namespace MyLittleLife.DataApi.Models;

public partial class ActionType
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public long NeedTypeId { get; set; }

    public decimal RestoreAmount { get; set; }

    public decimal DurationSeconds { get; set; }

    public virtual ICollection<ActionHistory> ActionHistories { get; set; } = new List<ActionHistory>();

    public virtual NeedType NeedType { get; set; } = null!;

    public virtual ICollection<FurnitureType> FurnitureTypes { get; set; } = new List<FurnitureType>();
}
