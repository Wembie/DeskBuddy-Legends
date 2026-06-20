using DeskBuddyLegends.Core.Domain.ValueObjects;

namespace DeskBuddyLegends.Core.Domain.Exceptions;

public sealed class CompanionNotFoundException : DomainException
{
    public CompanionId CompanionId { get; }

    public CompanionNotFoundException(CompanionId companionId)
        : base($"Companion '{companionId}' not found.")
    {
        CompanionId = companionId;
    }
}
