namespace DeskBuddyLegends.Core.Application.Interfaces;

public sealed record ActivityBucket(
    long Keystrokes,
    long MouseMoves,
    long MouseClicks,
    long Scrolls,
    DateTime BucketStartUtc,
    TimeSpan Duration
);

public interface IActivityMonitor : IDisposable
{
    event EventHandler<ActivityBucket>? BucketReady;
    event EventHandler<bool>? ActiveStateChanged;

    bool UserIsActive { get; }
    void Start();
    void Stop();
}
