using DeskBuddyLegends.Core.Domain.ValueObjects;

namespace DeskBuddyLegends.Core.Domain.Events;

public sealed record CapsuleItemResult(
    string ItemId,
    string ItemType,
    RarityTier Rarity
);

public sealed record CapsuleOpenedEvent(
    PlayerId PlayerId,
    string CapsuleType,
    IReadOnlyList<CapsuleItemResult> ItemsReceived
) : DomainEvent;
