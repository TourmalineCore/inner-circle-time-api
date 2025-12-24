using System.ComponentModel.DataAnnotations;
using Api.Features.Tracking.CreateWorkEntry;
using Api.Features.Tracking.GetWorkEntriesByPeriod;
using Api.Features.Tracking.UpdateWorkEntry;
using Application.Commands;
using Application.ExternalDeps.AssignmentsApi;
using Core.Entities;
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
        [Required][FromQuery] DateOnly startDate,
        [Required][FromQuery] DateOnly endDate,
        [FromServices] GetWorkEntriesByPeriodHandler getWorkEntriesByPeriodHandler
    )
    {
        return getWorkEntriesByPeriodHandler.HandleAsync(startDate, endDate);
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

    [EndpointSummary("Get employee projects by date")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpGet("projects")]
    public Task<ProjectsResponse> GetEmployeeProjectsAsync(
        [Required][FromQuery] DateOnly date,
        [FromServices] IAssignmentsApi assignmentsApi
    )
    {
        return assignmentsApi.GetEmployeeProjectsAsync(date);
    }

    [EndpointSummary("Deletes specific work entry")]
    [RequiresPermission(UserClaimsProvider.AUTO_TESTS_ONLY_IsWorkEntriesHardDeleteAllowed)]
    [HttpDelete("{workEntryId}/hard-delete")]
    public async Task<object> HardDeleteWorkEntryAsync(
        [Required][FromRoute] long workEntryId,
        [FromServices] HardDeleteEntityCommand hardDeleteEntityCommand
    )
    {
        return new
        {
            isDeleted = await hardDeleteEntityCommand.ExecuteAsync<WorkEntry>(workEntryId)
        };
    }
}
