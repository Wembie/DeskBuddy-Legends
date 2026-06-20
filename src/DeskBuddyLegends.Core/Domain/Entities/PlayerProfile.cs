using DeskBuddyLegends.Core.Domain.Events;
using DeskBuddyLegends.Core.Domain.ValueObjects;
using DeskBuddyLegends.Core.SharedKernel;

namespace DeskBuddyLegends.Core.Domain.Entities;

public sealed class PlayerProfile
{
    private readonly List<IDomainEvent> _domainEvents = [];
    private readonly HashSet<CompanionId> _unlockedCompanions = [];
    private readonly HashSet<AchievementId> _unlockedAchievements = [];

    public PlayerId Id { get; }
    public int PlayerLevel { get; private set; }
    public XpAmount TotalXp { get; private set; }
    public long TotalSessionSeconds { get; private set; }
    public long TotalKeystrokes { get; private set; }
    public long TotalMouseEvents { get; private set; }
    public int DailyLoginStreak { get; private set; }
    public DateTime FirstPlayedUtc { get; }
    public DateTime LastSeenUtc { get; private set; }
    public CompanionId? ActiveCompanionId { get; private set; }
    public IReadOnlyCollection<CompanionId> UnlockedCompanions => _unlockedCompanions;
    public IReadOnlyCollection<AchievementId> UnlockedAchievements => _unlockedAchievements;
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private const int MaxPlayerLevel = 500;

    public PlayerProfile(PlayerId id)
    {
        Id = Guard.NotNull(id);
        PlayerLevel = 1;
        TotalXp = XpAmount.Zero;
        FirstPlayedUtc = DateTime.UtcNow;
        LastSeenUtc = DateTime.UtcNow;
        DailyLoginStreak = 1;
    }

    public void RecordSession(long durationSeconds, long keystrokes, long mouseEvents)
    {
        if (durationSeconds < 0) return;

        TotalSessionSeconds += durationSeconds;
        TotalKeystrokes += Math.Max(0, keystrokes);
        TotalMouseEvents += Math.Max(0, mouseEvents);

        LastSeenUtc = DateTime.UtcNow;
    }

    public void AddXp(XpAmount amount)
    {
        TotalXp = TotalXp.Add(amount);
        TryLevelUp();
    }

    public void UnlockCompanion(CompanionId companionId)
    {
        Guard.NotNull(companionId);
        _unlockedCompanions.Add(companionId);
    }

    public void SetActiveCompanion(CompanionId companionId)
    {
        Guard.NotNull(companionId);
        ActiveCompanionId = companionId;
    }

    public bool UnlockAchievement(AchievementId achievementId, string steamApiKey)
    {
        Guard.NotNull(achievementId);

        if (_unlockedAchievements.Contains(achievementId)) return false;

        _unlockedAchievements.Add(achievementId);
        _domainEvents.Add(new AchievementUnlockedEvent(Id, achievementId, steamApiKey));
        return true;
    }

    public bool HasAchievement(AchievementId achievementId) =>
        _unlockedAchievements.Contains(achievementId);

    public void UpdateLoginStreak(DateOnly today)
    {
        var lastDate = DateOnly.FromDateTime(LastSeenUtc);
        DailyLoginStreak = today == lastDate.AddDays(1) ? DailyLoginStreak + 1 : 1;
        LastSeenUtc = DateTime.UtcNow;
    }

    public void ClearDomainEvents() => _domainEvents.Clear();

    private void TryLevelUp()
    {
        while (PlayerLevel < MaxPlayerLevel && TotalXp >= XpRequiredForLevel(PlayerLevel + 1))
            PlayerLevel++;
    }

    public static XpAmount XpRequiredForLevel(int level)
    {
        Guard.Positive(level);
        return new XpAmount((long)(500 * Math.Pow(level - 1, 1.8)));
    }
}
