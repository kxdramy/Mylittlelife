using System;
using System.Collections.Generic;

namespace MyLittleLife.DataApi.Models;

public partial class Room
{
    public long Id { get; set; }

    public long SaveSlotId { get; set; }

    public string Name { get; set; } = null!;

    public string RoomType { get; set; } = null!;

    public virtual ICollection<Furniture> Furnitures { get; set; } = new List<Furniture>();

    public virtual ICollection<RoomSurface> RoomSurfaces { get; set; } = new List<RoomSurface>();

    public virtual SaveSlot SaveSlot { get; set; } = null!;
}
