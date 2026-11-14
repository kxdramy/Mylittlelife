using System;
using System.Collections.Generic;

namespace MyLittleLife.DataApi.Models;

public partial class Character
{
    public long Id { get; set; }

    public long SaveSlotId { get; set; }

    public string Name { get; set; } = null!;

    public float PositionX { get; set; }

    public float PositionY { get; set; }

    public float PositionZ { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual CharacterAppearance? CharacterAppearance { get; set; }

    public virtual ICollection<CharacterNeed> CharacterNeeds { get; set; } = new List<CharacterNeed>();

    public virtual SaveSlot SaveSlot { get; set; } = null!;
}
