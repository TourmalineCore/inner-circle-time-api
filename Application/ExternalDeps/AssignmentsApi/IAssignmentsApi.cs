namespace Application.ExternalDeps.AssignmentsApi;

public interface IAssignmentsApi
{
    Task<ProjectsResponse> GetEmployeeProjectsByPeriodAsync(DateOnly startDate, DateOnly endDate);

    Task<ProjectDto> GetEmployeeProjectAsync(long projectId);
}
