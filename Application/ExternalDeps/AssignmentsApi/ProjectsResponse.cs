namespace Application.ExternalDeps.AssignmentsApi;

public class ProjectsResponse
{
    public required List<ProjectDto> Projects { get; set; }
}

public class ProjectDto
{
    public required long Id { get; set; }

    public required string Name { get; set; }
}
