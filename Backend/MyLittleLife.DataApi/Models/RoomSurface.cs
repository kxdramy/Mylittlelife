using System;
using System.Collections.Generic;

namespace MyLittleLife.DataApi.Models;

public partial class RoomSurface
{
    public long RoomId { get; set; }

    public string SurfaceKind { get; set; } = null!;

    public long SurfaceMaterialId { get; set; }

    public virtual Room Room { get; set; } = null!;

    public virtual SurfaceMaterial SurfaceMaterial { get; set; } = null!;
}
