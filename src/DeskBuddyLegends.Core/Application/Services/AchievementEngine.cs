using DeskBuddyLegends.Core.Domain.Entities;
using DeskBuddyLegends.Core.Domain.Events;
using DeskBuddyLegends.Core.Domain.ValueObjects;

namespace DeskBuddyLegends.Core.Application.Services;

/// <summary>
/// Routes domain events to metric-indexed achievements for O(relevant) evaluation.
/// All 500+ achievements loaded at boot; only achievements tied to the triggered
/// metric are evaluated per event. No hardcoded achievement logic.
/// </summary>
public sealed class AchievementEngine
{
    private readonly Dictionary<string, List<Achievement>> byMetric = new(StringComparer.Ordinal);
    private readonly Dictionary<AchievementId, Achievement> all = [];
    private PlayerProfile? playerProfile;

    public int TotalLoaded => all.Count;

    public void LoadDefinitions(IEnumerable<Achievement> achievements)
    {
        byMetric.Clear();
        all.Clear();

        foreach (var achievement in achievements)
        {
            all[achievement.Id] = achievement;

            var metric = achievement.Criteria.Metric;
            if (!byMetric.TryGetValue(metric, out var bucket))
            {
                bucket = [];
                byMetric[metric] = bucket;
            }

            bucket.Add(achievement);
        }
    }

    public void SetPlayerContext(PlayerProfile profile) => playerProfile = profile;

    public IReadOnlyList<Achievement> Handle(IDomainEvent domainEvent)
    {
        if (playerProfile is null) return [];

        var metrics = ExtractMetrics(domainEvent);
        var newlyUnlocked = new List<Achievement>();

        foreach (var (metric, value) in metrics)
        {
            if (!byMetric.TryGetValue(metric, out var candidates)) continue;

            foreach (var achievement in candidates)
            {
                if (achievement.IsUnlocked) continue;
                if (playerProfile.HasAchievement(achievement.Id)) continue;

                var unlocked = achievement.TryUnlock(value);
                if (unlocked)
                    newlyUnlocked.Add(achievement);
                else
                    achievement.UpdateProgress(value);
            }
        }

        return newlyUnlocked.AsReadOnly();
    }

    public AchievementProgress? GetProgress(AchievementId id) =>
        all.TryGetValue(id, out var a) ? a.Progress : null;

    public IReadOnlyList<Achievement> GetAll() => all.Values.ToList().AsReadOnly();

    private IEnumerable<(string Metric, double Value)> ExtractMetrics(IDomainEvent evt)
    {
        if (playerProfile is null) yield break;

        switch (evt)
        {
            case XpAwardedEvent e:
                yield return ("TotalXp", e.TotalXp.Value);
                break;

            case CompanionLeveledUpEvent e:
                yield return ("CompanionLevel", e.NewLevel);
                break;

            case CompanionEvolvedEvent e:
                yield return ("EvolutionStage", e.NewStage.StageIndex);
                break;

            case AchievementUnlockedEvent:
                yield return ("TotalAchievements", playerProfile.UnlockedAchievements.Count);
                break;

            case CapsuleOpenedEvent:
                yield return ("TotalCapsulesOpened", 1);
                break;

            case ActivitySessionCompletedEvent e:
                yield return ("TotalKeystrokes", playerProfile.TotalKeystrokes);
                yield return ("TotalMouseEvents", playerProfile.TotalMouseEvents);
                yield return ("SessionDurationSeconds", e.Duration.TotalSeconds);
                yield return ("TotalSessionSeconds", playerProfile.TotalSessionSeconds);
                break;
        }
    }
}
