using Application.ExternalDeps.AssignmentsApi;

namespace Api.ExternalDeps.AssignmentsApi;

internal class AssignmentsApi : IAssignmentsApi
{
    public AssignmentsApi()
    {
    }

    // for now it is implemented as a mock but later it will be an internal request to assignments-api
    // notes for future:
    // method will return employee projects by date
    // we will extract token inside method and pass it to assignments-api by which that api will understand which employee we should return the projects for
    public async Task<ProjectsResponse> GetEmployeeProjectsAsync(DateOnly date)
    {
        return new ProjectsResponse
        {
            Projects =
            [
                new Project { Id = 1, Name = "L1" },
                new Project { Id = 2, Name = "L2" },
                new Project { Id = 3, Name = "L3" },
                new Project { Id = 4, Name = "Inner Circle" },
                new Project { Id = 5, Name = "Sensei" },
                new Project { Id = 6, Name = "ML" },
                new Project { Id = 7, Name = "Branding" },
                new Project { Id = 8, Name = "Infrastructure" },
                new Project { Id = 9, Name = "Operations" },
                new Project { Id = 10, Name = "Management" }
            ]
        };
    }
}
