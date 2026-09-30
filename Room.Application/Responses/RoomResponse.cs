namespace Room.Application.Responses;

public class RoomResponse<T>
{
    public bool IsSuccessful { get; private set; }

    public string Message { get; private set; }

    public T? Data { get; private set; }

    private RoomResponse(bool isSuccessful, string message, T? data)
    {
        IsSuccessful = isSuccessful;
        Message = message;
        Data = data;
    }

    public static RoomResponse<T> Success(T data) => new(true, string.Empty, data);

    public static RoomResponse<T> Failure(string message) => new(false, message, default);
}