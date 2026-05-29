public static class MinutesConverterExtensions
{
    public static decimal ToHoursWithoutRounding(this int minutes)
    {
        return minutes / 60m;
    }
}
