using DeskBuddyLegends.Core.Domain.ValueObjects;

namespace DeskBuddyLegends.Core.Domain.Events;

public sealed record EmotionalStateChangedEvent(
    CompanionId CompanionId,
    EmotionalState PreviousState,
    EmotionalState NewState
) : DomainEvent;
