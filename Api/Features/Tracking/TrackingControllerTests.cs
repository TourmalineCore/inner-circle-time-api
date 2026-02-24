using Api.Features.Tracking.CreateTaskEntry;
using Application.TestsConfig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Api.Features.Tracking;

[IntegrationTest]
public class TrackingControllerTests : HttpClientTestBase
{
    public TrackingControllerTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateTaskEntryAsync_ShouldThrowValidationErrorIfAtLeastOneOfRequiredFieldsIsEmpty()
    {
        var createTaskEntryRequest = new CreateTaskEntryRequest
        {
            Title = "",
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
            TaskId = "#2231",
            ProjectId = 1,
            Description = "Task description",
        };

        var response = await HttpClient.PostAsJsonAsync("/api/time/tracking/task-entries", createTaskEntryRequest);

        Assert.NotNull(response);
        Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);

        var validationProblemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(validationProblemDetails);
        Assert.Equal("Fill in all the fields", validationProblemDetails.Detail);
        Assert.Equal("Validation error", validationProblemDetails.Title);
    }
}
