namespace TGParser.API.Utils;

public static class RussianPluralizationUtils
{
    public static string GetDayWord(int value)
    {
        if (value % 100 is >= 11 and <= 14) return "дней";
        return (value % 10) switch
        {
            1 => "день",
            2 or 3 or 4 => "дня",
            _ => "дней"
        };
    }

    public static string GetHourWord(int value)
    {
        if (value % 100 is >= 11 and <= 14) return "часов";
        return (value % 10) switch
        {
            1 => "час",
            2 or 3 or 4 => "часа",
            _ => "часов"
        };
    }

    public static string GetMinuteWord(int value)
    {
        if (value % 100 is >= 11 and <= 14) return "минут";
        return (value % 10) switch
        {
            1 => "минута",
            2 or 3 or 4 => "минуты",
            _ => "минут"
        };
    }
}
