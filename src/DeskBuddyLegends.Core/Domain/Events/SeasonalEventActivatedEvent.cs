namespace DeskBuddyLegends.Core.Domain.Events;

public sealed record SeasonalEventActivatedEvent(
    string EventId,
    string EventName,
    DateTime EndsAt
) : DomainEvent;
