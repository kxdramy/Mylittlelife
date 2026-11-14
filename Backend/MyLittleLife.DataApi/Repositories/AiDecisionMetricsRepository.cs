
using Microsoft.EntityFrameworkCore;
using MyLittleLife.DataApi.Data;
using MyLittleLife.DataApi.Models;

namespace MyLittleLife.DataApi.Repositories;

// Интерфейс для работы с историей решений ИИ.
public interface IAiDecisionMetricsRepository
{
    // Сохранение нового решения.
    Task<long> SaveDecisionAsync(
        long characterId,
        string actionName,
        string strategyCode,
        double utilityScore,
        double decisionTimeMs);

    // Получение последних решений персонажа.
    Task<List<AiDecisionMetric>> GetRecentDecisionsAsync(
        long characterId,
        int limit = 50);
}

// Реализация Repository через Entity Framework Core.
public class AiDecisionMetricsRepository
    : IAiDecisionMetricsRepository
{
    private readonly AiSettingsDataContext _db;

    public AiDecisionMetricsRepository(
        AiSettingsDataContext db)
    {
        _db = db;
    }

    // Записываем решение Utility AI в PostgreSQL.
    public async Task<long> SaveDecisionAsync(
        long characterId,
        string actionName,
        string strategyCode,
        double utilityScore,
        double decisionTimeMs)
    {
        AiDecisionMetric metric = new AiDecisionMetric
        {
            CharacterId = characterId,
            ActionName = actionName,
            StrategyCode = strategyCode,
            UtilityScore = utilityScore,
            DecisionTimeMs = decisionTimeMs,
            CreatedAt = DateTime.UtcNow
        };

        // Добавляем новую запись в таблицу.
        _db.AiDecisionMetrics.Add(metric);

        // Сохраняем изменения в PostgreSQL.
        await _db.SaveChangesAsync();

        // Возвращаем ID созданной записи.
        return metric.Id;
    }

    // Получаем последние решения персонажа.
    public async Task<List<AiDecisionMetric>>
        GetRecentDecisionsAsync(
            long characterId,
            int limit = 50)
    {
        return await _db.AiDecisionMetrics
            .AsNoTracking()
            .Where(m => m.CharacterId == characterId)
            .OrderByDescending(m => m.CreatedAt)
            .ThenByDescending(m => m.Id)
            .Take(limit)
            .ToListAsync();
    }
}
