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
    private readonly Dictionary<string, List<Achievement>> _byMetric = new(StringComparer.Ordinal);
    private readonly Dictionary<AchievementId, Achievement> _all = [];
    private PlayerProfile? _playerProfile;

    public int TotalLoaded => _all.Count;

    public void LoadDefinitions(IEnumerable<Achievement> achievements)
    {
        _byMetric.Clear();
        _all.Clear();

        foreach (var achievement in achievements)
        {
            _all[achievement.Id] = achievement;

            var metric = achievement.Criteria.Metric;
            if (!_byMetric.TryGetValue(metric, out var bucket))
            {
                bucket = [];
                _byMetric[metric] = bucket;
            }

            bucket.Add(achievement);
        }
    }

    public void SetPlayerContext(PlayerProfile profile) => _playerProfile = profile;

    public IReadOnlyList<Achievement> Handle(IDomainEvent domainEvent)
    {
        if (_playerProfile is null) return [];

        var metrics = ExtractMetrics(domainEvent);
        var newlyUnlocked = new List<Achievement>();

        foreach (var (metric, value) in metrics)
        {
            if (!_byMetric.TryGetValue(metric, out var candidates)) continue;

            foreach (var achievement in candidates)
            {
                if (achievement.IsUnlocked) continue;
                if (_playerProfile.HasAchievement(achievement.Id)) continue;

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
        _all.TryGetValue(id, out var a) ? a.Progress : null;

    public IReadOnlyList<Achievement> GetAll() => _all.Values.ToList().AsReadOnly();

    private IEnumerable<(string Metric, double Value)> ExtractMetrics(IDomainEvent evt)
    {
        if (_playerProfile is null) yield break;

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
                yield return ("TotalAchievements", _playerProfile.UnlockedAchievements.Count);
                break;

            case CapsuleOpenedEvent:
                yield return ("TotalCapsulesOpened", 1);
                break;

            case ActivitySessionCompletedEvent e:
                yield return ("TotalKeystrokes", _playerProfile.TotalKeystrokes);
                yield return ("TotalMouseEvents", _playerProfile.TotalMouseEvents);
                yield return ("SessionDurationSeconds", e.Duration.TotalSeconds);
                yield return ("TotalSessionSeconds", _playerProfile.TotalSessionSeconds);
                break;
        }
    }
}
