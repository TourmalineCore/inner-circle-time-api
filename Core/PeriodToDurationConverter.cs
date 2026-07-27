using Core;

public class PeriodToDurationConverter
{
    public Duration ConvertToDuration(Period period)
    {
        return new Duration
        {
            StartTime = period
                .StartDate
                .ToDateTime(TimeOnly.MinValue),
            EndTime = period
                .EndDate
                .AddDays(1)
                .ToDateTime(TimeOnly.MinValue)
        };
    }
}
