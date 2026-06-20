using DeskBuddyLegends.Core.SharedKernel;

namespace DeskBuddyLegends.Core.Domain.ValueObjects;

public sealed record XpAmount
{
    public long Value { get; }

    public static readonly XpAmount Zero = new(0);

    public XpAmount(long value)
    {
        Value = Guard.NonNegative(value);
    }

    public XpAmount Add(XpAmount other) => new(Value + other.Value);
    public XpAmount Subtract(XpAmount other) => new(Math.Max(0, Value - other.Value));
    public XpAmount Multiply(double multiplier) => new((long)Math.Floor(Value * Math.Max(0, multiplier)));

    public bool IsGreaterThan(XpAmount other) => Value > other.Value;
    public bool IsZero => Value == 0;

    public static XpAmount operator +(XpAmount a, XpAmount b) => a.Add(b);
    public static bool operator >(XpAmount a, XpAmount b) => a.Value > b.Value;
    public static bool operator <(XpAmount a, XpAmount b) => a.Value < b.Value;
    public static bool operator >=(XpAmount a, XpAmount b) => a.Value >= b.Value;
    public static bool operator <=(XpAmount a, XpAmount b) => a.Value <= b.Value;

    public override string ToString() => $"{Value:N0} XP";
}
