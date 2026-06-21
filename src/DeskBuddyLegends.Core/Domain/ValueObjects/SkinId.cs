using DeskBuddyLegends.Core.SharedKernel;

namespace DeskBuddyLegends.Core.Domain.ValueObjects;

public sealed record SkinId
{
    public static readonly SkinId Default = new("default");

    public string Value { get; }

    public SkinId(string value)
    {
        Value = Guard.NotNullOrWhiteSpace(value);
    }

    public static SkinId From(string value) => new(value);

    public bool IsDefault => Value == "default";

    public override string ToString() => Value;
}
