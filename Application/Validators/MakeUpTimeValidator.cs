namespace Application.Validators;

public class MakeUpTimeValidator
{
    public static bool DoesMakeUpTotalTimeMatchWithRelatedEntryPeriod(
        DateTime startTime,
        DateTime endTime,
        List<MakeUpTimeEntryDto> makeUpTimeList
    )
    {
        var totalRelatedEntryPeriodMinutes = (int)(endTime - startTime).TotalMinutes;
        var totalMakeUpTimeEntriesMinutes = makeUpTimeList.Sum(x => (int)(x.EndTime - x.StartTime).TotalMinutes);

        return totalMakeUpTimeEntriesMinutes == totalRelatedEntryPeriodMinutes;
    }
}
