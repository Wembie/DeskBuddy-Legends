using DeskBuddyLegends.Core.SharedKernel;

namespace DeskBuddyLegends.Core.Application.Interfaces;

public sealed record SteamAchievementInfo(string ApiKey, bool IsUnlocked, DateTime? UnlockedAt);

public interface ISteamAchievements
{
    Task<Result> UnlockAsync(string steamApiKey, CancellationToken ct = default);
    Task<bool> IsUnlockedAsync(string steamApiKey, CancellationToken ct = default);
    Task<Result> StoreStatsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<SteamAchievementInfo>> GetAllAsync(CancellationToken ct = default);
}
