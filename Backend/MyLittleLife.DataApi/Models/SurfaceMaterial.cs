using System;
using System.Collections.Generic;

namespace MyLittleLife.DataApi.Models;

public partial class SurfaceMaterial
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string SurfaceKind { get; set; } = null!;

    public string AssetKey { get; set; } = null!;

    public virtual ICollection<RoomSurface> RoomSurfaces { get; set; } = new List<RoomSurface>();
}
