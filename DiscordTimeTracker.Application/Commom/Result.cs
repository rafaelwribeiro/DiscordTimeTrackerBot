namespace DiscordTimeTracker.Application.Common;

public class Result
{
    public bool IsSuccess { get; }
    public string Error { get; }

    public bool IsFailure => !IsSuccess;

    protected Result(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error ?? throw new ArgumentNullException(nameof(error));
    }

    public static Result Ok() => new Result(true, string.Empty);

    public static Result Fail(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("Error message must not be empty.", nameof(error));

        return new Result(false, error);
    }
}

public class Result<T> : Result
{
    private readonly T? _value;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value when result is failure.");

    protected Result(T? value, bool isSuccess, string error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public static Result<T> Ok(T value)
    {
        return new Result<T>(value, true, string.Empty);
    }

    public static new Result<T> Fail(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("Error message must not be empty.", nameof(error));

        return new Result<T>(default, false, error);
    }
}
