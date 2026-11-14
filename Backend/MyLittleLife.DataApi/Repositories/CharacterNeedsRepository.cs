
using Microsoft.EntityFrameworkCore;
using MyLittleLife.DataApi.Data;
using MyLittleLife.DataApi.Models;

namespace MyLittleLife.DataApi.Repositories;

public record NeedValueDto(long NeedTypeId, decimal Value);

public interface ICharacterNeedsRepository
{
    Task<List<NeedValueDto>> GetNeedsAsync(long characterId);

    Task SaveNeedsAsync(
        long characterId,
        IReadOnlyList<NeedValueDto> values);
}

public class CharacterNeedsRepository
    : ICharacterNeedsRepository
{
    private readonly GameDataContext _db;

    public CharacterNeedsRepository(GameDataContext db)
    {
        _db = db;
    }

    public async Task<List<NeedValueDto>> GetNeedsAsync(
        long characterId)
    {
        return await _db.Set<CharacterNeed>()
            .AsNoTracking()
            .Where(n => n.CharacterId == characterId)
            .OrderBy(n => n.NeedTypeId)
            .Select(n => new NeedValueDto(
                n.NeedTypeId,
                n.Value))
            .ToListAsync();
    }

    public async Task SaveNeedsAsync(
        long characterId,
        IReadOnlyList<NeedValueDto> values)
    {
        var existingNeeds = await _db.Set<CharacterNeed>()
            .Where(n => n.CharacterId == characterId)
            .ToDictionaryAsync(n => n.NeedTypeId);

        foreach (var item in values)
        {
            if (existingNeeds.TryGetValue(
                item.NeedTypeId, out var existing))
            {
                // Обновляем существующую потребность.
                existing.Value = item.Value;
            }
            else
            {
                // Создаём запись, если её ещё нет.
                _db.Set<CharacterNeed>().Add(
                    new CharacterNeed
                    {
                        CharacterId = characterId,
                        NeedTypeId = item.NeedTypeId,
                        Value = item.Value
                    });
            }
        }

        await _db.SaveChangesAsync();
    }
}
