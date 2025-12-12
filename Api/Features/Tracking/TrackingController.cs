using System.ComponentModel.DataAnnotations;
using Api.Features.Tracking.CreateWorkEntry;
using Api.Features.Tracking.GetWorkEntriesByPeriod;
using Api.Features.Tracking.UpdateWorkEntry;
using Application.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Filters;

namespace Api.Features.Tracking;

[Authorize]
[ApiController]
[Route("api/time/tracking/work-entries")]
public class TrackingController : ControllerBase
{
    [EndpointSummary("Get work entries by period")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpGet]
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
    [HttpPost]
    public Task<CreateWorkEntryResponse> CreateWorkEntryAsync(
        [Required][FromBody] CreateWorkEntryRequest createWorkEntryRequest,
        [FromServices] CreateWorkEntryHandler createWorkEntryHandler
    )
    {
        return createWorkEntryHandler.HandleAsync(createWorkEntryRequest);
    }

    [EndpointSummary("Update a work entry")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpPost("{workEntryId}")]
    public Task UpdateWorkEntryAsync(
        [Required][FromRoute] long workEntryId,
        [Required][FromBody] UpdateWorkEntryRequest updateWorkEntryRequest,
        [FromServices] UpdateWorkEntryHandler updateWorkEntryHandler
    )
    {
        return updateWorkEntryHandler.HandleAsync(workEntryId, updateWorkEntryRequest);
    }

    [EndpointSummary("Deletes specific work entry")]
    [RequiresPermission(UserClaimsProvider.AUTO_TESTS_ONLY_IsWorkEntriesHardDeleteAllowed)]
    [HttpDelete("{workEntryId}/hard-delete")]
    public async Task<object> HardDeleteWorkEntryAsync(
        [Required][FromRoute] long workEntryId,
        [FromServices] HardDeleteWorkEntryCommand hardDeleteWorkEntryCommand
    )
    {
        return new
        {
            isDeleted = await hardDeleteWorkEntryCommand.ExecuteAsync(workEntryId)
        };
    }
}
