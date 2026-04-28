public static class DateTimeExtensions
{
    public static decimal GetHours(this DateTime startTime, DateTime endTime)
    {
        return startTime.GetTotalMinutes(endTime) / 60;
    }

    public static int GetTotalMinutes(this DateTime startTime, DateTime endTime)
    {
        return (int)(endTime - startTime).TotalMinutes;
    }
}
