using System;
using Core.Entities;

namespace Application.Extensions;

public static class TrackedEntryBaseQueryableExtensions
{
    public static IQueryable<TEntry> FilterByPeriod<TEntry>(
        this IQueryable<TEntry> queryable,
        DateOnly startDate,
        DateOnly endDate
    ) where TEntry : TrackedEntryBase
    {
        // Todo: Think about how to test date filtering
        // Issue: https://github.com/orgs/TourmalineCore/projects/5/views/1?pane=issue&itemId=171292110&issue=TourmalineCore%7Cinner-circle-time-api%7C69
        return queryable
            .Where(x => x.StartTime >= startDate.ToDateTime(TimeOnly.MinValue) && x.EndTime <= endDate.ToDateTime(TimeOnly.MaxValue));
    }
}
