using DeskBuddyLegends.Core.Domain.ValueObjects;
using DeskBuddyLegends.Core.SharedKernel;

namespace DeskBuddyLegends.Core.Domain.Entities;

public sealed class ActivitySession
{
    public Guid SessionId { get; }
    public PlayerId PlayerId { get; }
    public DateTime StartUtc { get; }
    public DateTime EndUtc { get; private set; }
    public long Keystrokes { get; private set; }
    public long MouseMoveEvents { get; private set; }
    public long MouseClickEvents { get; private set; }
    public long ScrollEvents { get; private set; }
    public GenuineScore Score { get; private set; }
    public bool IsFlagged { get; private set; }

    public TimeSpan Duration => EndUtc - StartUtc;
    public bool IsActive => EndUtc == StartUtc;

    public ActivitySession(PlayerId playerId)
    {
        SessionId = Guid.NewGuid();
        PlayerId = Guard.NotNull(playerId);
        StartUtc = DateTime.UtcNow;
        EndUtc = StartUtc;
        Score = GenuineScore.Perfect;
    }

    public void RecordKeystroke() => Keystrokes++;
    public void RecordMouseMove() => MouseMoveEvents++;
    public void RecordMouseClick() => MouseClickEvents++;
    public void RecordScroll() => ScrollEvents++;

    public void Complete(GenuineScore score)
    {
        if (!IsActive) return;

        EndUtc = DateTime.UtcNow;
        Score = Guard.NotNull(score);
        IsFlagged = score.IsSuspicious;
    }

    public long TotalEvents => Keystrokes + MouseMoveEvents + MouseClickEvents + ScrollEvents;
}
