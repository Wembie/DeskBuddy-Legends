using DeskBuddyLegends.Core.Domain.ValueObjects;

namespace DeskBuddyLegends.Core.Application.Services;

/// <summary>
/// Multi-layer anti-abuse scoring. Produces a GenuineScore [0,1] from raw timing data.
/// Layer 1: Shannon entropy of inter-event timing (low entropy = bot-like)
/// Layer 2: Repeated pattern detection via sliding window hash
/// Layer 3: Burst rate cap (>25 events/sec is physiologically impossible sustained)
/// </summary>
public sealed class AntiAbuseService
{
    private const int MaxEventsPerSecond = 25;
    private const double MinEntropyThreshold = 1.5;
    private const int PatternWindowSize = 20;
    private const int PatternRepeatLimit = 3;

    public GenuineScore ComputeScore(IReadOnlyList<long> interEventTimingsMs)
    {
        if (interEventTimingsMs.Count == 0)
            return new GenuineScore(0.5);

        var entropyScore = ComputeEntropyScore(interEventTimingsMs);
        var patternScore = ComputePatternScore(interEventTimingsMs);
        var burstScore = ComputeBurstScore(interEventTimingsMs);

        // Weighted geometric mean — any layer can veto the session
        var composite = entropyScore * patternScore * burstScore;
        return GenuineScore.Clamp(Math.Pow(composite, 1.0 / 3.0));
    }

    private static double ComputeEntropyScore(IReadOnlyList<long> timings)
    {
        if (timings.Count < 5) return 0.7;

        var buckets = new Dictionary<long, int>();
        foreach (var t in timings)
        {
            var bucket = t / 10;
            buckets[bucket] = buckets.GetValueOrDefault(bucket) + 1;
        }

        var total = (double)timings.Count;
        var entropy = 0.0;
        foreach (var count in buckets.Values)
        {
            var p = count / total;
            if (p > 0) entropy -= p * Math.Log2(p);
        }

        return Math.Min(1.0, entropy / MinEntropyThreshold);
    }

    private static double ComputePatternScore(IReadOnlyList<long> timings)
    {
        if (timings.Count < PatternWindowSize) return 1.0;

        var windowHashes = new List<int>();
        for (var i = 0; i <= timings.Count - PatternWindowSize; i++)
        {
            var hash = ComputeWindowHash(timings, i, PatternWindowSize);
            windowHashes.Add(hash);
        }

        var hashCounts = new Dictionary<int, int>();
        foreach (var h in windowHashes)
            hashCounts[h] = hashCounts.GetValueOrDefault(h) + 1;

        var maxRepeat = hashCounts.Values.DefaultIfEmpty(0).Max();
        return maxRepeat >= PatternRepeatLimit ? 0.1 : 1.0;
    }

    private static double ComputeBurstScore(IReadOnlyList<long> timings)
    {
        var minIntervalMs = 1000.0 / MaxEventsPerSecond;
        var burstCount = timings.Count(t => t < minIntervalMs);
        var burstRatio = (double)burstCount / timings.Count;
        return burstRatio > 0.5 ? 0.2 : 1.0 - (burstRatio * 0.5);
    }

    private static int ComputeWindowHash(IReadOnlyList<long> timings, int start, int length)
    {
        var hash = new HashCode();
        for (var i = start; i < start + length; i++)
            hash.Add(timings[i] / 5); // Quantize to 5ms buckets to allow minor variation
        return hash.ToHashCode();
    }
}
