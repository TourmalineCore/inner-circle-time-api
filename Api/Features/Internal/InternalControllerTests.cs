using Application.ExternalDeps.AssignmentsApi;
using Application.Features.Internal.GetEmployeesTrackedTaskHoursByProject;
using Application.Features.Tracking.CreateTaskEntry;
using Application.Features.Tracking.HardDeleteEntry;
using Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Api.Features.Internal;

[IntegrationTest]
public class InternalControllerTests : HttpClientTestBase
{
    public InternalControllerTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetProjects_ShouldReturnCorrectResponse()
    {
        var response = await HttpClient.GetFromJsonAsync<ProjectsResponse>("/internal/projects");

        Assert.NotNull(response);
        Assert.NotEmpty(response.Projects);
    }

    [Fact]
    public async Task GetTrackedTaskHoursByProject_ShouldReturnCorrectResponse()
    {
        var createTaskEntryRequest = new CreateTaskEntryRequest
        {
            Title = "Title",
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 11, 0, 0),
            TaskId = "#2231",
            ProjectId = 1,
            Description = "Task description",
        };

        var createTaskEntryResponse = await HttpClient.PostAsJsonAsync("/api/tracking/task-entries", createTaskEntryRequest);

        var newTaskEntryId = createTaskEntryResponse.Content.ReadFromJsonAsync<CreateTaskEntryResponse>()?.Result!.NewTaskEntryId;

        var trackedTaskHoursResponse = await HttpClient.GetFromJsonAsync<GetEmployeesTrackedTaskHoursByProjectResponse>($"/internal/projects/tracked-task-hours?projectId={createTaskEntryRequest.ProjectId}&startDate={DateOnly.FromDateTime(createTaskEntryRequest.StartTime)}&endDate={DateOnly.FromDateTime(createTaskEntryRequest.EndTime)}");

        Assert.NotNull(trackedTaskHoursResponse);
        Assert.Equal(2, trackedTaskHoursResponse.EmployeesTrackedTaskHours[0].TrackedHours);

        var deleteEntriesResponse = await HttpClient.DeleteFromJsonAsync<HardDeleteEntryResponse>($"/api/tracking/entries/{newTaskEntryId}/hard-delete");

        Assert.NotNull(deleteEntriesResponse);
        Assert.True(deleteEntriesResponse.IsDeleted);
    }
}
