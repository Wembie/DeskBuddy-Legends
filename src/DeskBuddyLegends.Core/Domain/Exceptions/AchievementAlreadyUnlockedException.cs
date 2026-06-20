using DeskBuddyLegends.Core.Domain.ValueObjects;

namespace DeskBuddyLegends.Core.Domain.Exceptions;

public sealed class AchievementAlreadyUnlockedException : DomainException
{
    public AchievementId AchievementId { get; }

    public AchievementAlreadyUnlockedException(AchievementId achievementId)
        : base($"Achievement '{achievementId}' is already unlocked.")
    {
        AchievementId = achievementId;
    }
}
