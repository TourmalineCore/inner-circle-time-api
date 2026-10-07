using Application.SharedDtos;

namespace Application.ExternalDeps.AssignmentsApi;

public class ProjectsResponse
{
    public required List<ProjectDto> Projects { get; set; }
}
