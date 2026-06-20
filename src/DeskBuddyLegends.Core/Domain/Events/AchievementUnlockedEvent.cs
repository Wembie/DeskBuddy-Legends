using DeskBuddyLegends.Core.Domain.ValueObjects;

namespace DeskBuddyLegends.Core.Domain.Events;

public sealed record AchievementUnlockedEvent(
    PlayerId PlayerId,
    AchievementId AchievementId,
    string SteamApiKey
) : DomainEvent;
