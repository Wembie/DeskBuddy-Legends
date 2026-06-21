using DeskBuddyLegends.Core.SharedKernel;

namespace DeskBuddyLegends.Core.Domain.ValueObjects;

public sealed record EvolutionStage
{
    public static readonly EvolutionStage Base = new(0, "Base");

    public int StageIndex { get; }
    public string StageName { get; }

    public EvolutionStage(int stageIndex, string stageName)
    {
        if (stageIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(stageIndex), "Stage index cannot be negative.");
        StageIndex = stageIndex;
        StageName = Guard.NotNullOrWhiteSpace(stageName);
    }

    public bool IsBase => StageIndex == 0;
    public bool IsEvolved => StageIndex > 0;

    public override string ToString() => $"Stage {StageIndex}: {StageName}";
}
