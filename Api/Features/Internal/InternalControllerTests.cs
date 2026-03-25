using Application.ExternalDeps.AssignmentsApi;
using Application.Features.Tracking.CreateTaskEntry;
using Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Api.Features.Internal;

[IntegrationTest]
public class InternalControllerTests : HttpClientTestBase
{
    public InternalControllerTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetProjects_ShouldReturnCorrectResponse()
    {
        var response = await HttpClient.GetFromJsonAsync<ProjectsResponse>("/internal/projects");

        Assert.NotNull(response);
        Assert.NotEmpty(response.Projects);
    }
}
