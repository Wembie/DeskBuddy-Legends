using DeskBuddyLegends.Core.Domain.ValueObjects;

namespace DeskBuddyLegends.Core.Domain.Exceptions;

public sealed class EvolutionConditionsNotMetException : DomainException
{
    public CompanionId CompanionId { get; }

    public EvolutionConditionsNotMetException(CompanionId companionId, string reason)
        : base($"Companion '{companionId}' cannot evolve: {reason}")
    {
        CompanionId = companionId;
    }
}
