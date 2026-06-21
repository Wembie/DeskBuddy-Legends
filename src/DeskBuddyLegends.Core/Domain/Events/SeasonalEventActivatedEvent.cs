namespace DeskBuddyLegends.Core.Domain.Events;

public sealed record SeasonalEventActivatedEvent(
    string SeasonalEventId,
    string EventName,
    DateTime EndsAt
) : DomainEvent;
