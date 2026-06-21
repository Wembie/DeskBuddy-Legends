using DeskBuddyLegends.Core.Domain.Entities;
using DeskBuddyLegends.Core.Domain.ValueObjects;

namespace DeskBuddyLegends.Core.Domain.Repositories;

public interface IAchievementRepository
{
    Task<IReadOnlyList<Achievement>> GetAllDefinitionsAsync(CancellationToken ct = default);
    Task<Achievement?> GetByIdAsync(AchievementId id, CancellationToken ct = default);
    Task SaveProgressAsync(Achievement achievement, CancellationToken ct = default);
    Task SaveProgressManyAsync(IEnumerable<Achievement> achievements, CancellationToken ct = default);
    Task<IReadOnlyList<Achievement>> GetUnlockedAsync(CancellationToken ct = default);
}
