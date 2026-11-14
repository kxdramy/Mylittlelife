
using Microsoft.EntityFrameworkCore;
using MyLittleLife.DataApi.Data;
using MyLittleLife.DataApi.Models;

namespace MyLittleLife.DataApi.Repositories;

// Интерфейс для работы с настройками Utility AI.
public interface IAiSettingsRepository
{
    Task<string?> GetStrategyAsync(long characterId);

    Task SaveStrategyAsync(
        long characterId,
        string strategyCode);
}

// Реализация Repository через Entity Framework Core.
public class AiSettingsRepository : IAiSettingsRepository
{
    private readonly AiSettingsDataContext _db;

    public AiSettingsRepository(
        AiSettingsDataContext db)
    {
        _db = db;
    }

    // Получаем сохранённую стратегию персонажа.
    public async Task<string?> GetStrategyAsync(
        long characterId)
    {
        return await _db.AiAgentSettings
            .AsNoTracking()
            .Where(s => s.CharacterId == characterId)
            .Select(s => s.StrategyCode)
            .FirstOrDefaultAsync();
    }

    // Сохраняем выбранную стратегию.
    public async Task SaveStrategyAsync(
        long characterId,
        string strategyCode)
    {
        // Разрешаем только две существующие стратегии.
        if (strategyCode != "FirstOnTie" &&
            strategyCode != "LastOnTie")
        {
            throw new ArgumentException(
                "Неизвестная стратегия Utility AI.");
        }

        // Проверяем, есть ли настройки персонажа в базе.
        var settings = await _db.AiAgentSettings
            .FindAsync(characterId);

        if (settings == null)
        {
            // Если записи нет — создаём новую.
            settings = new AiAgentSetting
            {
                CharacterId = characterId,
                StrategyCode = strategyCode,
                UpdatedAt = DateTime.UtcNow
            };

            _db.AiAgentSettings.Add(settings);
        }
        else
        {
            // Если запись существует — обновляем.
            settings.StrategyCode = strategyCode;
            settings.UpdatedAt = DateTime.UtcNow;
        }

        // Записываем изменения в PostgreSQL.
        await _db.SaveChangesAsync();
    }
}
