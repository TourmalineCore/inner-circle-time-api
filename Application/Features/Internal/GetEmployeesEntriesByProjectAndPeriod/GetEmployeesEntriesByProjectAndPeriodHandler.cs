using Core.Entities;

namespace Application.Features.Internal.GetEmployeesEntriesByProjectAndPeriod;

public class GetEmployeesEntriesByProjectAndPeriodHandler
{
    private readonly GetEmployeesEntriesByProjectAndPeriodQuery _getEmployeesEntriesByProjectAndPeriodQuery;

    public GetEmployeesEntriesByProjectAndPeriodHandler(
        GetEmployeesEntriesByProjectAndPeriodQuery getEmployeesEntriesByProjectAndPeriodQuery
    )
    {
        _getEmployeesEntriesByProjectAndPeriodQuery = getEmployeesEntriesByProjectAndPeriodQuery;
    }

    public async Task<GetEmployeesEntriesByProjectAndPeriodResponse> HandleAsync(
        long projectId,
        DateOnly startDate,
        DateOnly endDate
    )
    {
        var employeesEntriesByProjectAndPeriod = await _getEmployeesEntriesByProjectAndPeriodQuery.GetByProjectAndPeriodAsync<TaskEntry>(
            projectId,
            startDate,
            endDate
        );

        var employeesEntries = employeesEntriesByProjectAndPeriod
            .OfType<TaskEntry>()
            .Select(
                x => new EmployeesEntriesDto
                {
                    EmployeeId = x.EmployeeId,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime
                })
                .ToList();

        return new GetEmployeesEntriesByProjectAndPeriodResponse
        {
            EmployeesEntries = employeesEntries
        };
    }
}
