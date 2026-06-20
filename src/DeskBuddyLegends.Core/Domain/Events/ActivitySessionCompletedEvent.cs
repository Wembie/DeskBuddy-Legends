using DeskBuddyLegends.Core.Domain.ValueObjects;

namespace DeskBuddyLegends.Core.Domain.Events;

public sealed record ActivitySessionCompletedEvent(
    PlayerId PlayerId,
    long Keystrokes,
    long MouseMoveEvents,
    long MouseClickEvents,
    TimeSpan Duration,
    GenuineScore Score,
    bool IsFlagged
) : DomainEvent;
