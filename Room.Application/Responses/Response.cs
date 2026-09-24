namespace Room.Application.Responses;

public class Response<T>
{
    public bool IsSuccessful { get; private set; }

    public string Message { get; private set; }

    public T? Data { get; private set; }

    private Response(bool isSuccessful, string message, T? data)
    {
        IsSuccessful = isSuccessful;
        Message = message;
        Data = data;
    }

    public static Response<T> Success(T data) => new(true, string.Empty, data);

    public static Response<T> Failure(string message) => new(false, message, default);
}