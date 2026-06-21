using DeskBuddyLegends.Core.SharedKernel;

namespace DeskBuddyLegends.Core.Domain.ValueObjects;

public sealed record PlayerId
{
    public Guid Value { get; }

    public PlayerId(Guid value)
    {
        Value = Guard.NotDefault(value);
    }

    public static PlayerId New() => new(Guid.NewGuid());
    public static PlayerId From(string value) => new(Guid.Parse(value));

    public override string ToString() => Value.ToString();
}
