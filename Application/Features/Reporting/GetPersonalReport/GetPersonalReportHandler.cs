using Application.ExternalDeps.AssignmentsApi;
using Core;
using Core.Entities;

namespace Application.Features.Reporting.GetPersonalReport;

public class GetPersonalReportHandler
{
    private GetEmployeeTrackedEntriesQuery _getEmployeeTrackedEntriesQuery;

    private IAssignmentsApi _assignmentsApi;

    public GetPersonalReportHandler(
        GetEmployeeTrackedEntriesQuery getEmployeeTrackedEntriesQuery,
        IAssignmentsApi assignmentsApi
    )
    {
        _getEmployeeTrackedEntriesQuery = getEmployeeTrackedEntriesQuery;
        _assignmentsApi = assignmentsApi;
    }

    public async Task<GetPersonalReportResponse> HandleAsync(
        long employeeId,
        int year,
        int month
    )
    {
        var startDate = DateOnly.Parse($"{year}-{month}-01");

        var lastDayOfMonth = DateTime.DaysInMonth(year, month);

        var endDate = DateOnly.Parse($"{year}-{month}-{lastDayOfMonth}");

        var projects = await _assignmentsApi.GetAllProjectsAsync();

        var employeeTrackedEntries = await _getEmployeeTrackedEntriesQuery.GetAsync(employeeId, startDate, endDate);

        var projectDict = projects.ToDictionary(x => x.Id);

        var taskEntries = employeeTrackedEntries
            .OfType<TaskEntry>()
            .Select(
                x => new TrackedEntryDto
                {
                    Id = x.Id,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Hours = x.GetDurationInHours(),
                    TrackedHoursPerDay = TotalTrackedMinutesPerDayCalculator.Calculate(employeeTrackedEntries, x.StartTime).ToHoursWithoutRounding(),
                    EntryType = x.Type,
                    Project = new ProjectDto
                    {
                        Id = x.ProjectId,
                        Name = projectDict[x.ProjectId].Name
                    },
                    Task = new TaskDto
                    {
                        Id = x.TaskId,
                        Title = x.Title
                    },
                    Description = x.Description
                })
                .ToList();

        var unwellEntries = employeeTrackedEntries
            .OfType<UnwellEntry>()
            .Select(
                x => new TrackedEntryDto
                {
                    Id = x.Id,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Hours = x.GetDurationInHours(),
                    TrackedHoursPerDay = TotalTrackedMinutesPerDayCalculator.Calculate(employeeTrackedEntries, x.StartTime).ToHoursWithoutRounding(),
                    EntryType = x.Type,
                    Project = null!,
                    Task = null!,
                    Description = null
                })
                .ToList();

        var sortedByDateAllEntries = taskEntries
            .Concat(unwellEntries)
            .OrderBy(e => e.StartTime)
            .ToList();

        var taskTotalMinutes = employeeTrackedEntries
            .OfType<TaskEntry>()
            .Sum(x => x.GetDurationInMinutes());

        var unwellTotalMinutes = employeeTrackedEntries
            .OfType<UnwellEntry>()
            .Sum(x => x.GetDurationInMinutes());

        return new GetPersonalReportResponse
        {
            TrackedEntries = sortedByDateAllEntries,
            TaskHours = taskTotalMinutes.ToHoursWithoutRounding(),
            UnwellHours = unwellTotalMinutes.ToHoursWithoutRounding()
        };
    }
}
