using DeskBuddyLegends.Core.SharedKernel;

namespace DeskBuddyLegends.Core.Domain.ValueObjects;

public enum RarityTier
{
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    Epic = 3,
    Legendary = 4,
    Mythic = 5,
    Celestial = 6,
    Void = 7
}

public sealed record Rarity
{
    public RarityTier Tier { get; }
    public double DropWeight { get; }
    public string DisplayName { get; }
    public string ColorHex { get; }

    private Rarity(RarityTier tier, double dropWeight, string displayName, string colorHex)
    {
        Tier = tier;
        DropWeight = dropWeight;
        DisplayName = displayName;
        ColorHex = colorHex;
    }

    public static readonly Rarity Common    = new(RarityTier.Common,    10000.0, "Common",    "#9E9E9E");
    public static readonly Rarity Uncommon  = new(RarityTier.Uncommon,  4000.0,  "Uncommon",  "#4CAF50");
    public static readonly Rarity Rare      = new(RarityTier.Rare,      1500.0,  "Rare",      "#2196F3");
    public static readonly Rarity Epic      = new(RarityTier.Epic,      400.0,   "Epic",      "#9C27B0");
    public static readonly Rarity Legendary = new(RarityTier.Legendary, 80.0,    "Legendary", "#FF9800");
    public static readonly Rarity Mythic    = new(RarityTier.Mythic,    15.0,    "Mythic",    "#F44336");
    public static readonly Rarity Celestial = new(RarityTier.Celestial, 4.0,     "Celestial", "#00BCD4");
    public static readonly Rarity Void      = new(RarityTier.Void,      1.0,     "Void",      "#212121");

    private static readonly Dictionary<RarityTier, Rarity> _registry = new()
    {
        [RarityTier.Common]    = Common,
        [RarityTier.Uncommon]  = Uncommon,
        [RarityTier.Rare]      = Rare,
        [RarityTier.Epic]      = Epic,
        [RarityTier.Legendary] = Legendary,
        [RarityTier.Mythic]    = Mythic,
        [RarityTier.Celestial] = Celestial,
        [RarityTier.Void]      = Void,
    };

    public static Rarity FromTier(RarityTier tier) =>
        _registry.TryGetValue(tier, out var rarity)
            ? rarity
            : throw new ArgumentOutOfRangeException(nameof(tier), $"Unknown rarity tier: {tier}");

    public static IReadOnlyCollection<Rarity> All => _registry.Values;

    public bool IsAtLeast(RarityTier minimumTier) => Tier >= minimumTier;

    public override string ToString() => DisplayName;
}
