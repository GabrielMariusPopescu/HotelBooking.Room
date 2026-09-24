namespace Room.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class EnumExtensions
{
    public static bool TryValidate<TEnum>(this string input, out string displayName) where TEnum
        : struct, Enum
    {
        displayName = string.Empty;

        if (!Enum.TryParse(input, out TEnum parsedValue))
        {
            return false;
        }

        var parsedDisplayName = parsedValue.GetDisplayName();
        var acceptedValues = typeof(TEnum).GetDisplayNames();

        if (!acceptedValues.Contains(parsedDisplayName))
        {
            return false;
        }

        displayName = parsedDisplayName;
        return true;
    }

    public static IEnumerable<string> GetDisplayNames(this Type type)
        => Enum
            .GetValues(type)
            .Cast<Enum>()
            .Select(@enum => @enum.GetDisplayName());

    public static string GetDisplayName(this Enum value)
    {
        var member = value.GetType()
            .GetMember(value.ToString())
            .FirstOrDefault();

        var attribute = member?
            .GetCustomAttribute<DisplayAttribute>();

        return attribute?.Name ?? value.ToString();
    }
}