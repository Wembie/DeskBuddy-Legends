using DeskBuddyLegends.Core.Domain.Events;
using DeskBuddyLegends.Core.Domain.Exceptions;
using DeskBuddyLegends.Core.Domain.ValueObjects;
using DeskBuddyLegends.Core.SharedKernel;

namespace DeskBuddyLegends.Core.Domain.Entities;

public sealed class Companion
{
    private readonly List<IDomainEvent> _domainEvents = [];
    private readonly HashSet<SkinId> _ownedSkins = [];

    public CompanionId Id { get; }
    public string Name { get; private set; }
    public Rarity Rarity { get; }
    public AffinityType Affinity { get; }
    public EvolutionStage Stage { get; private set; }
    public int Level { get; private set; }
    public XpAmount TotalXp { get; private set; }
    public EmotionalState EmotionalState { get; private set; }
    public SkinId ActiveSkin { get; private set; }
    public bool IsUnlocked { get; private set; }
    public DateTime LastInteractionUtc { get; private set; }
    public IReadOnlyCollection<SkinId> OwnedSkins => _ownedSkins;
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private const int MaxLevel = 100;

    private Companion() { }

    public Companion(
        CompanionId id,
        string name,
        Rarity rarity,
        AffinityType affinity,
        bool isUnlocked = false)
    {
        Id = Guard.NotNull(id);
        Name = Guard.NotNullOrWhiteSpace(name);
        Rarity = Guard.NotNull(rarity);
        Affinity = affinity;
        Stage = EvolutionStage.Base;
        Level = 1;
        TotalXp = XpAmount.Zero;
        EmotionalState = EmotionalState.Neutral;
        ActiveSkin = SkinId.Default;
        IsUnlocked = isUnlocked;
        LastInteractionUtc = DateTime.UtcNow;

        _ownedSkins.Add(SkinId.Default);
    }

    public void Unlock()
    {
        if (IsUnlocked) return;
        IsUnlocked = true;
        LastInteractionUtc = DateTime.UtcNow;
    }

    public void AwardXp(XpAmount amount)
    {
        if (!IsUnlocked) return;
        if (amount.IsZero) return;

        TotalXp = TotalXp.Add(amount);
        LastInteractionUtc = DateTime.UtcNow;

        _domainEvents.Add(new XpAwardedEvent(Id, amount, TotalXp));

        TryLevelUp();
    }

    public void UpdateEmotionalState(EmotionalState newState)
    {
        if (EmotionalState == newState) return;

        var previous = EmotionalState;
        EmotionalState = newState;
        LastInteractionUtc = DateTime.UtcNow;

        _domainEvents.Add(new EmotionalStateChangedEvent(Id, previous, newState));
    }

    public void EquipSkin(SkinId skinId)
    {
        Guard.NotNull(skinId);

        if (!_ownedSkins.Contains(skinId))
            throw new SkinNotOwnedException(skinId);

        ActiveSkin = skinId;
    }

    public void UnlockSkin(SkinId skinId)
    {
        Guard.NotNull(skinId);
        _ownedSkins.Add(skinId);
    }

    public void Evolve(EvolutionStage newStage)
    {
        Guard.NotNull(newStage);

        if (newStage.StageIndex <= Stage.StageIndex)
            throw new EvolutionConditionsNotMetException(Id,
                $"New stage {newStage.StageIndex} must be higher than current {Stage.StageIndex}.");

        var previous = Stage;
        Stage = newStage;

        _domainEvents.Add(new CompanionEvolvedEvent(Id, previous, newStage));
    }

    public void ClearDomainEvents() => _domainEvents.Clear();

    private void TryLevelUp()
    {
        if (Level >= MaxLevel) return;

        while (Level < MaxLevel && TotalXp >= XpRequiredForLevel(Level + 1))
        {
            var previous = Level;
            Level++;
            _domainEvents.Add(new CompanionLeveledUpEvent(Id, previous, Level));
        }
    }

    public static XpAmount XpRequiredForLevel(int level)
    {
        Guard.Positive(level);
        // Quadratic curve: level 2 = 100 XP, level 100 = ~980,100 XP
        return new XpAmount((long)(100 * Math.Pow(level - 1, 2)));
    }
}
