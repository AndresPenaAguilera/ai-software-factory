namespace AiSoftwareFactory.Domain.Common;

/// <summary>Non-generic result representing success or failure.</summary>
public class Result
{
    /// <summary>Indicates whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Indicates whether the operation failed.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>The error associated with a failure result.</summary>
    public Error Error { get; }

    /// <summary>Initialises a new result.</summary>
    protected Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>Returns a successful result.</summary>
    public static Result Success() => new(true, default!);

    /// <summary>Returns a failure result with the given error.</summary>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>Returns a successful result carrying a value.</summary>
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, default!);

    /// <summary>Returns a failure result for a typed result.</summary>
    public static Result<TValue> Failure<TValue>(Error error) => new(default!, false, error);
}

/// <summary>Typed result that carries a value on success.</summary>
public sealed class Result<TValue> : Result
{
    private readonly TValue _value;

    internal Result(TValue value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>The value; throws if the result is a failure.</summary>
    public TValue Value => IsSuccess
        ? _value
        : throw new InvalidOperationException("Cannot access Value of a failure result.");

    /// <summary>Allows a value to be implicitly lifted into a successful result.</summary>
    public static implicit operator Result<TValue>(TValue value) => Success(value);
}
