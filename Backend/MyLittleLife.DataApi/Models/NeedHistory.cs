using System;
using System.Collections.Generic;

namespace MyLittleLife.DataApi.Models;

public partial class NeedHistory
{
    public long Id { get; set; }

    public long GameSessionId { get; set; }

    public long NeedTypeId { get; set; }

    public decimal Value { get; set; }

    public DateTime RecordedAt { get; set; }

    public virtual GameSession GameSession { get; set; } = null!;

    public virtual NeedType NeedType { get; set; } = null!;
}
