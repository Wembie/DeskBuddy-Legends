using DeskBuddyLegends.Core.Domain.Entities;
using DeskBuddyLegends.Core.Domain.ValueObjects;

namespace DeskBuddyLegends.Core.Domain.Repositories;

public interface IPlayerProfileRepository
{
    Task<PlayerProfile?> GetAsync(CancellationToken ct = default);
    Task SaveAsync(PlayerProfile profile, CancellationToken ct = default);
}
