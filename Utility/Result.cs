namespace MWF_Web_Api.Utility;

public class Result<T>
{
    public bool IsSuccessful { get; init; }

    public T Value { get; set; }

    public string Message { get; set; } = string.Empty;

    public Exception? Exception { get; set; }

    protected Result(T value)
    {
        Value = value;
    }

    protected Result(T value, string msg) 
        : this(value)
    {
        Message = msg;
    }

    protected Result(T value, string msg, Exception exception) 
        : this(value, msg)
    {
        Exception = exception;
    }

    public static Result<T> Success(T value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return new Result<T>(value) { IsSuccessful = true };
    }

    public static Result<T?> NotSuccess(T? value, string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentNullException(nameof(message));
        }

        return new Result<T?>(value, message) { IsSuccessful = false };
    }

    public static Result<bool> Success()
    {
        return new Result<bool>(true) { IsSuccessful = true };
    }

    public static Result<bool> NotSuccess(string msg)
    {
        return new Result<bool>(false, msg) { IsSuccessful = false };
    }
}