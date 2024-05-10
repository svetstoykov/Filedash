using System.Text.Json.Serialization;

namespace Filedash.Domain.Common;

public class Result<T> : Result
{
    public Result()
    { }
    
    private Result(T data, bool isSuccessful = false, string message = null) : base(isSuccessful, message)
    {
        Data = data;
    }

    public T Data { get; init; }
    
    public static Result<T> Success(T data, string message = null) => new(data, true, message);

    public static Result<T> Failure(string message = null) => new(default, false, message);
}

public class Result
{
    public Result()
    { }
    
    protected Result(bool isSuccessful, string message = null)
    {
        IsSuccessful = isSuccessful;
        Message = message;
    }

    public bool IsSuccessful { get; init; }
    
    public string Message { get; init; }
    
    public static Result Success(string message = null) => new(true, message);

    public static Result Failure(string message = null) => new(false, message);
}