namespace DeskBuddyLegends.Core.SharedKernel;

public sealed class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public bool IsFailure => !IsSuccess;

    private Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);

    public static implicit operator Result(string error) => Failure(error);
}

public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }

    private readonly T? value;

    public T Value => IsSuccess
        ? value!
        : throw new InvalidOperationException($"Cannot access Value of a failed Result: {Error}");

    private Result(T val)
    {
        IsSuccess = true;
        value = val;
        Error = null;
    }

    private Result(string error)
    {
        IsSuccess = false;
        value = default;
        Error = error;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string error) => new(error);

    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(string error) => Failure(error);

    public Result<TOut> Map<TOut>(Func<T, TOut> mapper) =>
        IsSuccess ? Result<TOut>.Success(mapper(Value)) : Result<TOut>.Failure(Error!);

    public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> binder) =>
        IsSuccess ? binder(Value) : Result<TOut>.Failure(Error!);

    public T GetValueOrDefault(T defaultValue) => IsSuccess ? Value : defaultValue;
}
