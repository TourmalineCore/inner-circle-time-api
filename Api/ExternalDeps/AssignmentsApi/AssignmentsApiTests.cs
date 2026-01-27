using Application.TestsConfig;
using Xunit;

namespace Api.ExternalDeps.AssignmentsApi;

[UnitTest]
public class AssignmentsApiTests
{
    [Fact]
    public async Task GetEmployeeProjectAsync_ShouldThrowErrorIfProjectDoesNotExist()
    {
        var nonExistingProjectId = 999;
        var assignmentsApi = new AssignmentsApi();

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            async () => await assignmentsApi.GetEmployeeProjectAsync(nonExistingProjectId)
        );

        Assert.Contains($"This project id was not found: {nonExistingProjectId}", exception.Message);
    }
}
