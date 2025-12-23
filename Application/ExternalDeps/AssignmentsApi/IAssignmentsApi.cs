namespace Application.ExternalDeps.AssignmentsApi;

public interface IAssignmentsApi
{
    Task<ProjectsResponse> GetEmployeeProjectsAsync(DateOnly date);
}
