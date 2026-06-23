namespace Application.Validators;

public class MakeUpTimeValidator
{
    public static bool IsMakeUpTotalTimeConvergingWithPeriod(
        DateTime startTime,
        DateTime endTime,
        List<MakeUpTimeEntryDto> makeUpTimeList
    )
    {
        var totalPeriodMinutes = (endTime - startTime).TotalMinutes;
        var totalMakeUpTimeEntryMinutes = makeUpTimeList.Sum(x => (x.EndTime - x.StartTime).TotalMinutes);

        return totalMakeUpTimeEntryMinutes == totalPeriodMinutes;
    }
}
