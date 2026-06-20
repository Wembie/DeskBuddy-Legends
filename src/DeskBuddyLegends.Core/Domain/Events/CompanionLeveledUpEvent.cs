using DeskBuddyLegends.Core.Domain.ValueObjects;

namespace DeskBuddyLegends.Core.Domain.Events;

public sealed record CompanionLeveledUpEvent(
    CompanionId CompanionId,
    int PreviousLevel,
    int NewLevel
) : DomainEvent;
