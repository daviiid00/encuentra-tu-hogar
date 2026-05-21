namespace EncuentraTuHogar.Domain.Common;

public abstract record Result<T>
{
    public sealed record Success(T Value) : Result<T>;
    public sealed record Failure(string Error) : Result<T>;

    public Result<TResult> Map<TResult>(Func<T, TResult> map) =>
        this switch
        {
            Success(var val) => new Result<TResult>.Success(map(val)),
            Failure(var err) => new Result<TResult>.Failure(err),
            _ => throw new NotSupportedException()
        };

    public Result<TResult> FlatMap<TResult>(Func<T, Result<TResult>> map) =>
        this switch
        {
            Success(var val) => map(val),
            Failure(var err) => new Result<TResult>.Failure(err),
            _ => throw new NotSupportedException()
        };

    public async Task<Result<TResult>> FlatMapAsync<TResult>(Func<T, Task<Result<TResult>>> map) =>
        this switch
        {
            Success(var val) => await map(val),
            Failure(var err) => new Result<TResult>.Failure(err),
            _ => throw new NotSupportedException()
        };
}

public static class Result
{
    public static Result<T>.Success Success<T>(T value) => new(value);
    public static Result<T>.Failure Failure<T>(string error) => new(error);
}
