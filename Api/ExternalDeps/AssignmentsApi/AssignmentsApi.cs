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
    public ProjectsResponse GetEmployeeProjectsAsync(DateOnly date)
    {
        return ProjectsMock.ProjectsMockData;
    }

    public Project? FindEmployeeProjectAsync(long projectId)
    {
        return ProjectsMock.ProjectsMockData.Projects.Find(x => x.Id == projectId);
    }
}
