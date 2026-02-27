using System.ComponentModel.DataAnnotations;
using Application.ExternalDeps.AssignmentsApi;
using Application.Features.Tracking.CreateTaskEntry;
using Application.Features.Tracking.CreateUnwellEntry;
using Application.Features.Tracking.GetEntriesByPeriod;
using Application.Features.Tracking.HardDeleteEntry;
using Application.Features.Tracking.SoftDeleteEntry;
using Application.Features.Tracking.UpdateTaskEntry;
using Application.Features.Tracking.UpdateUnwellEntry;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Filters;

namespace Api.Features.Tracking;

[Authorize]
[ApiController]
[Route("api/time/tracking")]
public class TrackingController : ControllerBase
{
    [EndpointSummary("Get entries by period")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpGet("entries")]
    public Task<GetEntriesByPeriodResponse> GetEntriesByPeriodAsync(
      [Required][FromQuery] DateOnly startDate,
      [Required][FromQuery] DateOnly endDate,
      [FromServices] GetEntriesByPeriodHandler getEntriesByPeriodHandler
    )
    {
        return getEntriesByPeriodHandler.HandleAsync(startDate, endDate);
    }

    [EndpointSummary("Create a task entry")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpPost("task-entries")]
    public Task<CreateTaskEntryResponse> CreateTaskEntryAsync(
      [Required][FromBody] CreateTaskEntryRequest createTaskEntryRequest,
      [FromServices] CreateTaskEntryHandler createTaskEntryHandler
    )
    {
        return createTaskEntryHandler.HandleAsync(createTaskEntryRequest);
    }

    [EndpointSummary("Create an unwell entry")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpPost("unwell-entries")]
    public Task<CreateUnwellResponse> CreateUnwellEntryAsync(
        [Required][FromBody] CreateUnwellEntryRequest createUnwellRequest,
        [FromServices] CreateUnwellEntryHandler createUnwellEntryHandler
    )
    {
        return createUnwellEntryHandler.HandleAsync(createUnwellRequest);
    }

    [EndpointSummary("Update a task entry")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpPost("task-entries/{taskEntryId}")]
    public Task UpdateTaskEntryAsync(
        [Required][FromRoute] long taskEntryId,
        [Required][FromBody] UpdateTaskEntryRequest updateTaskEntryRequest,
        [FromServices] UpdateTaskEntryHandler updateTaskEntryHandler
    )
    {
        return updateTaskEntryHandler.HandleAsync(taskEntryId, updateTaskEntryRequest);
    }

    [EndpointSummary("Update an unwell entry")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpPost("unwell-entries/{unwellEntryId}")]
    public Task UpdateUnwellEntryAsync(
    [Required][FromRoute] long unwellEntryId,
    [Required][FromBody] UpdateUnwellEntryRequest updateUnwellEntryRequest,
    [FromServices] UpdateUnwellEntryHandler updateUnwellEntryHandler
    )
    {
        return updateUnwellEntryHandler.HandleAsync(unwellEntryId, updateUnwellEntryRequest);
    }

    [EndpointSummary("Get employee projects by period")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpGet("task-entries/projects")]
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

    [EndpointSummary("Deletes specific entry")]
    [RequiresPermission(UserClaimsProvider.AUTO_TESTS_ONLY_IsEntriesHardDeleteAllowed)]
    [HttpDelete("entries/{entryId}/hard-delete")]
    public async Task<object> HardDeleteEntryAsync(
    [Required][FromRoute] long entryId,
    [FromServices] HardDeleteEntryHandler hardDeleteEntryHandler
    )
    {
        return new
        {
            isDeleted = await hardDeleteEntryHandler.HandleAsync(entryId)
        };
    }

    [EndpointSummary("Soft deletes specific entry")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpDelete("entries/{entryId}/soft-delete")]
    public Task<object> SoftDeleteEntryAsync(
    [Required][FromRoute] long entryId,
    [Required][FromBody] SoftDeleteEntryRequest softDeleteEntryRequest,
    [FromServices] SoftDeleteEntryHandler softDeleteEntryHandler
    )
    {
        return softDeleteEntryHandler.HandleAsync(entryId, softDeleteEntryRequest);
    }
}
