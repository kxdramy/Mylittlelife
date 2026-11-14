using System;
using System.Collections.Generic;

namespace MyLittleLife.DataApi.Models;

public partial class ActionHistory
{
    public long Id { get; set; }

    public long GameSessionId { get; set; }

    public long FurnitureId { get; set; }

    public long ActionTypeId { get; set; }

    public string ControlMode { get; set; } = null!;

    public DateTime ExecutedAt { get; set; }

    public decimal DurationSeconds { get; set; }

    public virtual ActionType ActionType { get; set; } = null!;

    public virtual Furniture Furniture { get; set; } = null!;

    public virtual GameSession GameSession { get; set; } = null!;
}
