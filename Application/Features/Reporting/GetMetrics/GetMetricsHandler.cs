using Application.SharedQueries;
using Core.Features.Reporting;

namespace Application.Features.Reporting.GetMetrics;

public class GetMetricsHandler
{
    private readonly GetEmployeeTrackedEntriesQuery _getEmployeeTrackedEntriesQuery;

    private readonly IClaimsProvider _claimsProvider;

    public GetMetricsHandler(
        GetEmployeeTrackedEntriesQuery getEmployeeTrackedEntriesQuery,
        IClaimsProvider claimsProvider
    )
    {
        _getEmployeeTrackedEntriesQuery = getEmployeeTrackedEntriesQuery;
        _claimsProvider = claimsProvider;
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

        var metrics = MetricsCalculator.Calculate(employeeTrackedEntries);

        return new GetMetricsResponse
        {
            TrackedHours = metrics.TrackedHours,
        };
    }
}
