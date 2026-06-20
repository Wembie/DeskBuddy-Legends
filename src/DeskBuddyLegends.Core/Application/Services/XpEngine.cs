using DeskBuddyLegends.Core.Domain.ValueObjects;

namespace DeskBuddyLegends.Core.Application.Services;

public sealed record XpEngineConfig(
    long BaseXpPerBucket,
    long DailyXpCap,
    double SessionLengthBonusPerHour,
    double MaxSessionLengthBonus,
    double EventMultiplier
)
{
    public static XpEngineConfig Default => new(
        BaseXpPerBucket: 10,
        DailyXpCap: 5000,
        SessionLengthBonusPerHour: 0.1,
        MaxSessionLengthBonus: 0.5,
        EventMultiplier: 1.0
    );
}

public sealed class XpEngine
{
    private readonly XpEngineConfig _config;

    public XpEngine(XpEngineConfig? config = null)
    {
        _config = config ?? XpEngineConfig.Default;
    }

    /// <summary>
    /// Calculates XP for an activity bucket given the session's genuine score and duration.
    /// Returns XpAmount.Zero if the score is below the abuse threshold.
    /// </summary>
    public XpAmount Calculate(
        GenuineScore genuineScore,
        TimeSpan sessionDuration,
        long xpAwardedTodayAlready)
    {
        if (genuineScore.IsSuspicious)
            return XpAmount.Zero;

        var remainingDailyXp = _config.DailyXpCap - xpAwardedTodayAlready;
        if (remainingDailyXp <= 0)
            return XpAmount.Zero;

        var sessionHours = sessionDuration.TotalHours;
        var sessionBonus = Math.Min(
            sessionHours * _config.SessionLengthBonusPerHour,
            _config.MaxSessionLengthBonus
        );

        var rawXp = _config.BaseXpPerBucket
            * genuineScore.Value
            * (1.0 + sessionBonus)
            * _config.EventMultiplier;

        var awarded = (long)Math.Floor(rawXp);
        var capped = Math.Min(awarded, remainingDailyXp);

        return capped > 0 ? new XpAmount(capped) : XpAmount.Zero;
    }

    public bool IsDailyCapped(long xpAwardedToday) => xpAwardedToday >= _config.DailyXpCap;
}
