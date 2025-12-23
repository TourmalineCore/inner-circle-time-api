namespace Application.ExternalDeps.AssignmentsApi;

public interface IAssignmentsApi
{
    ProjectsResponse GetEmployeeProjectsAsync(DateOnly date);

    Project? FindEmployeeProjectAsync(long projectId);
}
