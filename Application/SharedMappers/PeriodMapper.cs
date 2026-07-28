namespace Application.SharedMappers;

public static class PeriodMapper
{
    public static PeriodDto MapToPeriodDto(DateTime startTime, DateTime endTime)
    {
        return new PeriodDto
        {
            StartDate = DateOnly.FromDateTime(startTime),
            // DB stores end_time as the start of the next day (see ADR #008 - https://github.com/TourmalineCore/inner-circle-documentation/blob/master/time-tracker/adrs/008-sick-leave-and-vacation-storage.md)
            // Subtract 1 day when displaying to show the correct end date on UI
            EndDate = DateOnly.FromDateTime(endTime.AddDays(-1))
        };
    }
}
