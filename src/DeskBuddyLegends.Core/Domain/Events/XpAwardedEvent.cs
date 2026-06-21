using DeskBuddyLegends.Core.Domain.ValueObjects;

namespace DeskBuddyLegends.Core.Domain.Events;

public sealed record XpAwardedEvent(
    CompanionId CompanionId,
    XpAmount AmountAwarded,
    XpAmount TotalXp
) : DomainEvent;
