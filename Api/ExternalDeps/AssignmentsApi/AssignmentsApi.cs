using Application.ExternalDeps.AssignmentsApi;

namespace Api.ExternalDeps.AssignmentsApi;

internal class AssignmentsApi : IAssignmentsApi
{
    private static readonly ProjectsResponse ProjectsMock = new ProjectsResponse
    {
        Projects =
        [
            new ProjectDto { Id = 1, Name = "L1" },
            new ProjectDto { Id = 2, Name = "L2" },
            new ProjectDto { Id = 3, Name = "L3" },
            new ProjectDto { Id = 4, Name = "Inner Circle" },
            new ProjectDto { Id = 5, Name = "Sensei" },
            new ProjectDto { Id = 6, Name = "ML" },
            new ProjectDto { Id = 7, Name = "Branding" },
            new ProjectDto { Id = 8, Name = "Infrastructure" },
            new ProjectDto { Id = 9, Name = "Operations" },
            new ProjectDto { Id = 10, Name = "Management" }
        ]
    };

    // for now it is implemented as a mock but later it will be an internal request to assignments-api
    // notes for future:
    // method will return employee projects by date
    // we will extract token inside method and pass it to assignments-api by which that api will understand which employee we should return the projects for
    public async Task<ProjectsResponse> GetEmployeeProjectsByPeriodAsync(
        DateOnly startDate,
        DateOnly endDate
    )
    {
        return await Task.FromResult(ProjectsMock);
    }

    public async Task<ProjectDto> GetEmployeeProjectAsync(long projectId)
    {
        var project = await Task.FromResult(ProjectsMock.Projects.Find(x => x.Id == projectId));

        if (project == null)
        {
            throw new ArgumentException($"This project id was not found: {projectId}");
        }

        return project;
    }
}
