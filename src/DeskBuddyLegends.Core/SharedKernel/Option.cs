namespace DeskBuddyLegends.Core.SharedKernel;

public sealed class Option<T>
{
    private readonly T? _value;

    public bool HasValue { get; }

    public T Value => HasValue
        ? _value!
        : throw new InvalidOperationException("Option has no value.");

    private Option() { HasValue = false; }
    private Option(T value) { _value = value; HasValue = true; }

    public static Option<T> Some(T value) => new(Guard.NotNull(value));
    public static Option<T> None() => new();

    public T GetValueOrDefault(T defaultValue) => HasValue ? _value! : defaultValue;

    public Option<TOut> Map<TOut>(Func<T, TOut> mapper) =>
        HasValue ? Option<TOut>.Some(mapper(_value!)) : Option<TOut>.None();

    public Option<TOut> Bind<TOut>(Func<T, Option<TOut>> binder) =>
        HasValue ? binder(_value!) : Option<TOut>.None();

    public static implicit operator Option<T>(T value) => Some(value);
}
