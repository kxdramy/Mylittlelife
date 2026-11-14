using System;
using System.Collections.Generic;

namespace MyLittleLife.DataApi.Models;

public partial class CharacterNeed
{
    public long CharacterId { get; set; }

    public long NeedTypeId { get; set; }

    public decimal Value { get; set; }

    public virtual Character Character { get; set; } = null!;

    public virtual NeedType NeedType { get; set; } = null!;
}
