namespace Application.ExternalDeps.AssignmentsApi;

public static class ProjectsMock
{
    public static readonly ProjectsResponse ProjectsMockData = new ProjectsResponse
    {
        Projects =
        [
            new Project { Id = 1, Name = "L1" },
            new Project { Id = 2, Name = "L2" },
            new Project { Id = 3, Name = "L3" },
            new Project { Id = 4, Name = "Inner Circle" },
            new Project { Id = 5, Name = "Sensei" },
            new Project { Id = 6, Name = "ML" },
            new Project { Id = 7, Name = "Branding" },
            new Project { Id = 8, Name = "Infrastructure" },
            new Project { Id = 9, Name = "Operations" },
            new Project { Id = 10, Name = "Management" }
        ]
    };
}