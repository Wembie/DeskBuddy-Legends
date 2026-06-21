namespace DeskBuddyLegends.Core.SharedKernel;

public sealed class Option<T>
{
    private readonly T? value;

    public bool HasValue { get; }

    public T Value => HasValue
        ? value!
        : throw new InvalidOperationException("Option has no value.");

    private Option() { HasValue = false; }
    private Option(T val) { value = val; HasValue = true; }

    public static Option<T> Some(T value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        return new(value);
    }

    public static Option<T> None() => new();

    public T GetValueOrDefault(T defaultValue) => HasValue ? value! : defaultValue;

    public Option<TOut> Map<TOut>(Func<T, TOut> mapper) =>
        HasValue ? Option<TOut>.Some(mapper(value!)) : Option<TOut>.None();

    public Option<TOut> Bind<TOut>(Func<T, Option<TOut>> binder) =>
        HasValue ? binder(value!) : Option<TOut>.None();

    public static implicit operator Option<T>(T value) => Some(value);
}
