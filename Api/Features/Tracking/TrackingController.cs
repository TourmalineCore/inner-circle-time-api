using System.ComponentModel.DataAnnotations;
using Api.Features.Tracking.CreateWorkEntry;
using Api.Features.Tracking.GetWorkEntriesByPeriod;
using Application;
using Application.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Filters;

namespace Api.Features.Tracking;

[Authorize]
[ApiController]
[Route("api/time/tracking")]
public class TrackingController : ControllerBase
{
    [EndpointSummary("Get work entries by period")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpGet("work-entries")]
    public Task<GetWorkEntriesByPeriodResponse> GetWorkEntriesByPeriodAsync(
        [Required][FromQuery] DateTime startTime,
        [Required][FromQuery] DateTime endTime,
        [FromServices] GetWorkEntriesByPeriodHandler getWorkEntriesByPeriodHandler
    )
    {
        return getWorkEntriesByPeriodHandler.HandleAsync(startTime, endTime);
    }

    [EndpointSummary("Create a work entry")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpPost("work-entries")]
    public Task<CreateWorkEntryResponse> CreateWorkEntryAsync(
        [FromServices] CreateWorkEntryHandler createWorkEntryHandler,
        [Required][FromBody] CreateWorkEntryRequest createWorkEntryRequest
    )
    {
        return createWorkEntryHandler.HandleAsync(createWorkEntryRequest);
    }

    [EndpointSummary("Deletes specific work entry")]
    [RequiresPermission(UserClaimsProvider.AUTO_TESTS_ONLY_IsWorkEntriesHardDeleteAllowed)]
    [HttpDelete("{workEntryId}/hard-delete")]
    public async Task<object> HardWorkEntryTypeAsync(
        [FromServices] HardDeleteWorkEntryCommand hardDeleteWorkEntryCommand,
        [Required][FromRoute] long workEntryId
    )
    {
        return new
        {
            isDeleted = await hardDeleteWorkEntryCommand.ExecuteAsync(workEntryId)
        };
    }
}
