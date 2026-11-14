
using System;

namespace MyLittleLife.DataApi.Models;

public class AiAgentSetting
{
    // ID персонажа в PostgreSQL.
    public long CharacterId { get; set; }

    // Выбранная стратегия Utility AI.
    public string StrategyCode { get; set; } =
        "FirstOnTie";

    // Время последнего изменения настройки.
    public DateTime UpdatedAt { get; set; } =
        DateTime.UtcNow;
}
