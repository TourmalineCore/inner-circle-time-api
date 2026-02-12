using System.ComponentModel.DataAnnotations;
using Api.Features.Tracking.CreateAdjustment;
using Api.Features.Tracking.CreateWorkEntry;
using Api.Features.Tracking.GetAdjustmentsByPeriod;
using Api.Features.Tracking.GetWorkEntriesByPeriod;
using Api.Features.Tracking.UpdateAdjustment;
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
[Route("api/time/tracking")]
public class TrackingController : ControllerBase
{
    [EndpointSummary("Get work entries by period")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpGet("work-entries")]
    public Task<GetWorkEntriesByPeriodResponse> GetWorkEntriesByPeriodAsync(
        [Required][FromQuery] DateOnly startDate,
        [Required][FromQuery] DateOnly endDate,
        [FromServices] GetWorkEntriesByPeriodHandler getWorkEntriesByPeriodHandler
    )
    {
        return getWorkEntriesByPeriodHandler.HandleAsync(startDate, endDate);
    }

    [EndpointSummary("Get an adjustments by period")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpGet("adjustments")]
    public Task<GetAdjustmentsByPeriodResponse> GetAdjustmentsByPeriodAsync(
        [Required][FromQuery] DateOnly startDate,
        [Required][FromQuery] DateOnly endDate,
        [FromServices] GetAdjustmentsByPeriodHandler getAdjustmentsByPeriodHandler
    )
    {
        return getAdjustmentsByPeriodHandler.HandleAsync(startDate, endDate);
    }

    [EndpointSummary("Create a work entry")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpPost("work-entries")]
    public Task<CreateWorkEntryResponse> CreateWorkEntryAsync(
        [Required][FromBody] CreateWorkEntryRequest createWorkEntryRequest,
        [FromServices] CreateWorkEntryHandler createWorkEntryHandler
    )
    {
        return createWorkEntryHandler.HandleAsync(createWorkEntryRequest);
    }

    [EndpointSummary("Create an adjustment")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpPost("adjustments")]
    public Task<CreateAdjustmentResponse> CreateAdjustmentAsync(
        [Required][FromBody] CreateAdjustmentRequest createAdjustmentRequest,
        [FromServices] CreateAdjustmentHandler createAdjustmentHandler
    )
    {
        return createAdjustmentHandler.HandleAsync(createAdjustmentRequest);
    }

    [EndpointSummary("Update a work entry")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpPost("work-entries/{workEntryId}")]
    public Task UpdateWorkEntryAsync(
        [Required][FromRoute] long workEntryId,
        [Required][FromBody] UpdateWorkEntryRequest updateWorkEntryRequest,
        [FromServices] UpdateWorkEntryHandler updateWorkEntryHandler
    )
    {
        return updateWorkEntryHandler.HandleAsync(workEntryId, updateWorkEntryRequest);
    }


    [EndpointSummary("Update an adjustment")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpPost("adjustments/{adjustmentId}")]
    public Task UpdateAdjustmentAsync(
        [Required][FromRoute] long adjustmentId,
        [Required][FromBody] UpdateAdjustmentRequest updateAdjustmentRequest,
        [FromServices] UpdateAdjustmentHandler updateAdjustmentHandler
    )
    {
        return updateAdjustmentHandler.HandleAsync(adjustmentId, updateAdjustmentRequest);
    }

    [EndpointSummary("Get employee projects by period")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpGet("work-entries/projects")]
    public async Task<ProjectsResponse> GetEmployeeProjectsByPeriodAsync(
        [Required][FromQuery] DateOnly startDate,
        [Required][FromQuery] DateOnly endDate,
        [FromServices] IAssignmentsApi assignmentsApi
    )
    {
        return new ProjectsResponse
        {
            Projects = await assignmentsApi.GetEmployeeProjectsByPeriodAsync(startDate, endDate)
        };
    }

    [EndpointSummary("Deletes specific work entry")]
    [RequiresPermission(UserClaimsProvider.AUTO_TESTS_ONLY_IsWorkEntriesHardDeleteAllowed)]
    [HttpDelete("work-entries/{workEntryId}/hard-delete")]
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

    [EndpointSummary("Deletes specific adjustment")]
    [RequiresPermission(UserClaimsProvider.AUTO_TESTS_ONLY_IsAdjustmentsHardDeleteAllowed)]
    [HttpDelete("adjustments/{adjustmentId}/hard-delete")]
    public async Task<object> HardDeleteAdjustmentAsync(
        [Required][FromRoute] long adjustmentId,
        [FromServices] HardDeleteEntityCommand hardDeleteEntityCommand
    )
    {
        return new
        {
            isDeleted = await hardDeleteEntityCommand.ExecuteAsync<Adjustment>(adjustmentId)
        };
    }
}
