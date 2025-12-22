namespace Api.ExternalDeps.AssignmentsApi.Responses;

public class ProjectsResponse
{
    public required List<Project> Projects { get; set; }
}

public class Project
{
    public required long Id { get; set; }

    public required string Name { get; set; }
}