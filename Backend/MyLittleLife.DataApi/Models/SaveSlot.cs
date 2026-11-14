using System;
using System.Collections.Generic;

namespace MyLittleLife.DataApi.Models;

public partial class SaveSlot
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime LastSavedAt { get; set; }

    public virtual Character? Character { get; set; }

    public virtual ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
