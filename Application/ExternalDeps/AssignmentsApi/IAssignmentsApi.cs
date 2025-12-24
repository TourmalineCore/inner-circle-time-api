namespace Application.ExternalDeps.AssignmentsApi;

public interface IAssignmentsApi
{
    Task<ProjectsResponse> GetEmployeeProjectsAsync(DateOnly date);

    Task<Project?> FindEmployeeProjectAsync(long projectId);
}
