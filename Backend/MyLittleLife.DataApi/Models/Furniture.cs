using System;
using System.Collections.Generic;

namespace MyLittleLife.DataApi.Models;

public partial class Furniture
{
    public long Id { get; set; }

    public long RoomId { get; set; }

    public long FurnitureTypeId { get; set; }

    public float PositionX { get; set; }

    public float PositionY { get; set; }

    public float PositionZ { get; set; }

    public float RotationY { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<ActionHistory> ActionHistories { get; set; } = new List<ActionHistory>();

    public virtual FurnitureCustomization? FurnitureCustomization { get; set; }

    public virtual FurnitureType FurnitureType { get; set; } = null!;

    public virtual Room Room { get; set; } = null!;
}
