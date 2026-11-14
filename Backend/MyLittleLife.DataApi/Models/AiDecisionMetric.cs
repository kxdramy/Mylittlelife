
using System;

namespace MyLittleLife.DataApi.Models;

public class AiDecisionMetric
{
    // Уникальный номер записи.
    public long Id { get; set; }

    // Какой персонаж принял решение.
    public long CharacterId { get; set; }

    // Выбранное действие: Eat, Sleep, Work и т.д.
    public string ActionName { get; set; } =
        string.Empty;

    // Стратегия выбора действия.
    public string StrategyCode { get; set; } =
        "FirstOnTie";

    // Оценка полезности выбранного действия.
    public double UtilityScore { get; set; }

    // Время работы алгоритма выбора в миллисекундах.
    public double DecisionTimeMs { get; set; }

    // Дата и время принятия решения.
    public DateTime CreatedAt { get; set; } =
        DateTime.UtcNow;
}
