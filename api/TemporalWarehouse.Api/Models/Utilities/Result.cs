

namespace TemporalWarehouse.Api.Models.Utilities;

public class Result<T>
{
    protected Result(bool isSuccess, Error error, T? value)
    {
        if ((isSuccess && error != Error.None) ||
            (!isSuccess && error == Error.None))
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
        Value = value;
    }


    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;
    public Error Error { get; }
    public T? Value { get; }
    public static Result<T> Success() => new(true, Error.None, default);
    public static Result<T> Failure(Error error) => new(false, error, default);
    public static Result<T> SetValue(T value) => new(true, Error.None, value);
}