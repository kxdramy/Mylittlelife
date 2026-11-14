using System;
using System.Collections.Generic;

namespace MyLittleLife.DataApi.Models;

public partial class CharacterAppearance
{
    public long CharacterId { get; set; }

    public long HairStyleId { get; set; }

    public long OutfitId { get; set; }

    public string SkinColor { get; set; } = null!;

    public string HairColor { get; set; } = null!;

    public string OutfitColor { get; set; } = null!;

    public virtual Character Character { get; set; } = null!;

    public virtual HairStyle HairStyle { get; set; } = null!;

    public virtual Outfit Outfit { get; set; } = null!;
}
