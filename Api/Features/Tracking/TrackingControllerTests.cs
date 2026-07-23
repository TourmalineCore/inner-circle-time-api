using Application.Features.Tracking.CreateAwayWithMakeUpTimeEntry;
using Application.Features.Tracking.CreateTaskEntry;
using Core;
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

        var response = await HttpClient.PostAsJsonAsync("/tracking/task-entries", createTaskEntryRequest);

        Assert.NotNull(response);
        Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);

        var validationProblemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(validationProblemDetails);
        Assert.Equal("Fill in all the fields", validationProblemDetails.Detail);
        Assert.Equal("Validation error", validationProblemDetails.Title);
    }

    [Fact]
    public async Task CreateAwayWithMakeUpTimeAsync_ShouldThrowArgumentExceptionWithProblemDetailsIfMakeUpTotalTimeDoesNotMatchWithRelatedEntryPeriod()
    {
        var createAwayWithMakeUpTimeEntryRequest = new CreateAwayWithMakeUpTimeEntryRequest
        {
            StartTime = new DateTime(2026, 11, 24, 10, 0, 0),
            EndTime = new DateTime(2026, 11, 24, 12, 0, 0),
            Description = "Description",
            MakeUpTimeList = [
                new CreateOrUpdateMakeUpTimeEntryDto
                    {
                        StartTime = new DateTime(2026, 11, 24, 17, 0, 0),
                        EndTime = new DateTime(2026, 11, 24, 18, 0, 0),
                    }
                ]
        };

        var response = await HttpClient.PostAsJsonAsync("/tracking/away-with-make-up-time-entries", createAwayWithMakeUpTimeEntryRequest);

        Assert.NotNull(response);
        Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);

        var validationProblemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(validationProblemDetails);
        Assert.Equal("Total make-up time must equal your away time. Please check and adjust your entries.", validationProblemDetails.Detail);
        Assert.Equal("Time does not match", validationProblemDetails.Title);
    }
}
