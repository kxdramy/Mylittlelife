using System;
using System.Collections.Generic;

namespace MyLittleLife.DataApi.Models;

public partial class FurnitureType
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string PrefabKey { get; set; } = null!;

    public virtual ICollection<Furniture> Furnitures { get; set; } = new List<Furniture>();

    public virtual ICollection<ActionType> ActionTypes { get; set; } = new List<ActionType>();
}
