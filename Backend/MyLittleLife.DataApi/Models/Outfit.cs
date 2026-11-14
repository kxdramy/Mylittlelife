using System;
using System.Collections.Generic;

namespace MyLittleLife.DataApi.Models;

public partial class Outfit
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string AssetKey { get; set; } = null!;

    public virtual ICollection<CharacterAppearance> CharacterAppearances { get; set; } = new List<CharacterAppearance>();
}
