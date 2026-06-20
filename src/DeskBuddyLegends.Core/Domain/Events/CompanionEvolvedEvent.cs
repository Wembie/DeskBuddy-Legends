using DeskBuddyLegends.Core.Domain.ValueObjects;

namespace DeskBuddyLegends.Core.Domain.Events;

public sealed record CompanionEvolvedEvent(
    CompanionId CompanionId,
    EvolutionStage PreviousStage,
    EvolutionStage NewStage
) : DomainEvent;
