using DeskBuddyLegends.Core.Domain.ValueObjects;

namespace DeskBuddyLegends.Core.Domain.Exceptions;

public sealed class SkinNotOwnedException : DomainException
{
    public SkinId SkinId { get; }

    public SkinNotOwnedException(SkinId skinId)
        : base($"Skin '{skinId}' is not owned and cannot be equipped.")
    {
        SkinId = skinId;
    }
}
