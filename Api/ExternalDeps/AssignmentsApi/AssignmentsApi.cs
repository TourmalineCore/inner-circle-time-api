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
    public async Task<ProjectsResponse> GetEmployeeProjectsByPeriodAsync(
        DateOnly startDate,
        DateOnly endDate
    )
    {
        return await Task.FromResult(ProjectsMock.ProjectsMockData);
    }

    public async Task<Project?> FindEmployeeProjectAsync(long projectId)
    {
        return await Task.FromResult(ProjectsMock.ProjectsMockData.Projects.Find(x => x.Id == projectId));
    }
}
