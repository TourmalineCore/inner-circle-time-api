using Application.ExternalDeps.AssignmentsApi;

namespace Api.ExternalDeps.AssignmentsApi;

internal class AssignmentsApi : IAssignmentsApi
{
    private static readonly List<ProjectDto> MockedProjects = new()
        {
            new ProjectDto { Id = 1 },
            new ProjectDto { Id = 2 },
            new ProjectDto { Id = 3 },
            new ProjectDto { Id = 4 },
            new ProjectDto { Id = 5 },
            new ProjectDto { Id = 6 },
            new ProjectDto { Id = 7 },
            new ProjectDto { Id = 8 },
            new ProjectDto { Id = 9 },
            new ProjectDto { Id = 10 },
            new ProjectDto { Id = 11 },
            new ProjectDto { Id = 12 },
            new ProjectDto { Id = 13 },
            new ProjectDto { Id = 14 },
            new ProjectDto { Id = 15 },
            new ProjectDto { Id = 16 },
            new ProjectDto { Id = 17 }
        };

    // for now it is implemented as a mock but later it will be an internal request to assignments-api
    // notes for future:
    // method will return employee projects by date
    // we will extract token inside method and pass it to assignments-api by which that api will understand which employee we should return the projects for
    public async Task<List<ProjectDto>> GetEmployeeProjectsByPeriodAsync(
        DateOnly startDate,
        DateOnly endDate
    )
    {
        return await Task.FromResult(MockedProjects);
    }

    public async Task<ProjectDto> GetEmployeeProjectAsync(long projectId)
    {
        var project = await Task.FromResult(MockedProjects.Find(x => x.Id == projectId));

        if (project == null)
        {
            throw new ArgumentException($"This project id was not found: {projectId}");
        }

        return project;
    }
}
