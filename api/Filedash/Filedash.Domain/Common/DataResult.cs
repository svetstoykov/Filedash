namespace Filedash.Domain.Common;

public class DataResult<T>
{
    internal DataResult(T data, bool isSuccessful = false, string message = null)
    {
        Data = data;
        Result = new Result(isSuccessful, message);
    }

    internal DataResult(T data, Result result)
    {
        Data = data;
        Result = result;
    }
    
    public T Data { get; init; }
    
    public Result Result { get; init; }
    
    public static DataResult<T> Success(T data, string message = null) => new(data, true, message);

    public static DataResult<T> Failure(string message = null) => new(default, false, message);

    public static DataResult<T> New(T data, Result result) => new(data, result);
}

public class Result
{
    public Result(bool isSuccessful, string message = null)
    {
        IsSuccessful = isSuccessful;
        Message = message;
    }

    public bool IsSuccessful { get; init; }
    
    public string Message { get; init; }
    
    public static Result Success(string message = null) => new(true, message);

    public static Result Failure(string message = null) => new(false, message);
}