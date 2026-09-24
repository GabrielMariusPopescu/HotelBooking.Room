namespace Room.Application.Extensions;

public static class ObjectExtensions
{
    public static Guid ToDeterministicGuid(this string value)
    {
        var inputBytes = Encoding.Default.GetBytes(value);
        var hashBytes = MD5.HashData(inputBytes);
        var input = Convert.ToHexString(hashBytes);
        return Guid.ParseExact(input, "N");
    }
}