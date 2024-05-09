namespace Filedash.Domain.Common;

public class DataResult<T> : Result
{
    private DataResult(T data, bool isSuccessful = false, string message = null) : base(isSuccessful, message)
    {
        Data = data;
    }

    public T Data { get; init; }
    
    public static DataResult<T> Success(T data, string message = null) => new(data, true, message);

    public static DataResult<T> Failure(string message = null) => new(default, false, message);
}

public class Result
{
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