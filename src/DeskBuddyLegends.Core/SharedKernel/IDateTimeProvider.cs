namespace DeskBuddyLegends.Core.SharedKernel;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateOnly Today { get; }
}
