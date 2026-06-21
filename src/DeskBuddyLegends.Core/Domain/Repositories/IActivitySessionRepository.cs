using DeskBuddyLegends.Core.Domain.Entities;

namespace DeskBuddyLegends.Core.Domain.Repositories;

public interface IActivitySessionRepository
{
    Task SaveAsync(ActivitySession session, CancellationToken ct = default);
    Task<IReadOnlyList<ActivitySession>> GetTodaySessionsAsync(CancellationToken ct = default);
    Task<long> GetTodayXpAwardedAsync(CancellationToken ct = default);
    Task SaveFlaggedSessionAsync(ActivitySession session, string reason, CancellationToken ct = default);
}
