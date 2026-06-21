namespace DeskBuddyLegends.Core.Domain.ValueObjects;

public enum EmotionalStateType
{
    Neutral = 0,
    Happy = 1,
    Excited = 2,
    Sad = 3,
    Curious = 4,
    Bored = 5,
    Tired = 6,
    Affectionate = 7,
    Sleeping = 8,
    Working = 9
}

public sealed record EmotionalState
{
    public EmotionalStateType Type { get; }
    public float Intensity { get; }
    public int DecayAfterSeconds { get; }

    private EmotionalState(EmotionalStateType type, float intensity, int decayAfterSeconds)
    {
        Type = type;
        Intensity = Math.Clamp(intensity, 0f, 1f);
        DecayAfterSeconds = decayAfterSeconds;
    }

    public static EmotionalState Neutral => new(EmotionalStateType.Neutral, 1.0f, -1);
    public static EmotionalState Happy => new(EmotionalStateType.Happy, 0.8f, 300);
    public static EmotionalState Excited => new(EmotionalStateType.Excited, 1.0f, 60);
    public static EmotionalState Sad => new(EmotionalStateType.Sad, 0.6f, -1);
    public static EmotionalState Curious => new(EmotionalStateType.Curious, 0.5f, 120);
    public static EmotionalState Bored => new(EmotionalStateType.Bored, 0.4f, -1);
    public static EmotionalState Tired => new(EmotionalStateType.Tired, 0.3f, -1);
    public static EmotionalState Affectionate => new(EmotionalStateType.Affectionate, 0.9f, 600);
    public static EmotionalState Sleeping => new(EmotionalStateType.Sleeping, 1.0f, -1);
    public static EmotionalState Working => new(EmotionalStateType.Working, 0.7f, 30);

    public bool IsTransient => DecayAfterSeconds > 0;

    public override string ToString() => Type.ToString();
}
