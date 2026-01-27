namespace Application.ExternalDeps.AssignmentsApi;

public interface IAssignmentsApi
{
    Task<List<ProjectDto>> GetEmployeeProjectsByPeriodAsync(DateOnly startDate, DateOnly endDate);

    Task<ProjectDto> GetEmployeeProjectAsync(long projectId);
}
