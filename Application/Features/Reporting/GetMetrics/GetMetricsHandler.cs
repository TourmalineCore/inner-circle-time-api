using Application.SharedQueries;
using Core.Entities;

namespace Application.Features.Reporting.GetMetrics;

public class GetMetricsHandler
{
    private readonly GetEmployeeTrackedEntriesQuery _getEmployeeTrackedEntriesQuery;

    private readonly IClaimsProvider _claimsProvider;

    public GetMetricsHandler(
        GetEmployeeTrackedEntriesQuery getEmployeeTrackedEntriesQuery,
        IClaimsProvider claimsProvide
    )
    {
        _getEmployeeTrackedEntriesQuery = getEmployeeTrackedEntriesQuery;
        _claimsProvider = claimsProvide;
    }

    public async Task<GetMetricsResponse> HandleAsync(
        DateOnly startDate,
        DateOnly endDate
    )
    {
        var employeeTrackedEntries = await _getEmployeeTrackedEntriesQuery.GetAsync(
            _claimsProvider.EmployeeId,
            startDate,
            endDate
        );

        var taskTotalMinutes = employeeTrackedEntries
            .OfType<TaskEntry>()
            .Sum(x => x.GetDurationInMinutes());

        var unwellTotalMinutes = employeeTrackedEntries
            .OfType<UnwellEntry>()
            .Sum(x => x.GetDurationInMinutes());

        var trackedHours = (taskTotalMinutes + unwellTotalMinutes).ToHoursWithoutRounding();

        return new GetMetricsResponse
        {
            TrackedHours = trackedHours,
        };
    }
}
