using DeskBuddyLegends.Core.Domain.Entities;
using DeskBuddyLegends.Core.Domain.ValueObjects;

namespace DeskBuddyLegends.Core.Domain.Repositories;

public interface ICompanionRepository
{
    Task<Companion?> GetByIdAsync(CompanionId id, CancellationToken ct = default);
    Task<IReadOnlyList<Companion>> GetUnlockedAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Companion>> GetAllAsync(CancellationToken ct = default);
    Task SaveAsync(Companion companion, CancellationToken ct = default);
    Task SaveManyAsync(IEnumerable<Companion> companions, CancellationToken ct = default);
}
