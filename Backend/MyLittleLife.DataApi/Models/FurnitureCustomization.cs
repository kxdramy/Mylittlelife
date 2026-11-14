using System;
using System.Collections.Generic;

namespace MyLittleLife.DataApi.Models;

public partial class FurnitureCustomization
{
    public long FurnitureId { get; set; }

    public string ColorHex { get; set; } = null!;

    public string? MaterialVariant { get; set; }

    public virtual Furniture Furniture { get; set; } = null!;
}
