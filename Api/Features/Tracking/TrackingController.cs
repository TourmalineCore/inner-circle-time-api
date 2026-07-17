using System.ComponentModel.DataAnnotations;
using Application.ExternalDeps.AssignmentsApi;
using Application.Features.Tracking.CreateAwayWithMakeUpTimeEntry;
using Application.Features.Tracking.CreateSickLeaveEntry;
using Application.Features.Tracking.CreateTaskEntry;
using Application.Features.Tracking.CreateUnwellEntry;
using Application.Features.Tracking.GetAwayWithMakeUpTimeEntry;
using Application.Features.Tracking.GetEntriesByPeriod;
using Application.Features.Tracking.GetSickLeaveEntry;
using Application.Features.Tracking.GetTaskEntry;
using Application.Features.Tracking.GetUnwellEntry;
using Application.Features.Tracking.HardDeleteEntry;
using Application.Features.Tracking.SoftDeleteEntry;
using Application.Features.Tracking.UpdateAwayWithMakeUpTimeEntry;
using Application.Features.Tracking.UpdateSickLeaveEntry;
using Application.Features.Tracking.UpdateTaskEntry;
using Application.Features.Tracking.UpdateUnwellEntry;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Filters;

namespace Api.Features.Tracking;

[Authorize]
[ApiController]
[Route("api/tracking")]
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

    [EndpointSummary("Get a task entry")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpGet("task-entries/{taskEntryId}")]
    public Task<GetTaskEntryResponse> GetTaskEntryAsync(
        [Required][FromRoute] long taskEntryId,
        [FromServices] GetTaskEntryHandler getTaskEntryHandler
    )
    {
        return getTaskEntryHandler.HandleAsync(taskEntryId);
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

    [EndpointSummary("Get an unwell entry")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpGet("unwell-entries/{unwellEntryId}")]
    public Task<GetUnwellEntryResponse> GetUnwellEntryAsync(
      [Required][FromRoute] long unwellEntryId,
      [FromServices] GetUnwellEntryHandler getUnwellEntryHandler
    )
    {
        return getUnwellEntryHandler.HandleAsync(unwellEntryId);
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

    [EndpointSummary("Get an away with make up time entry")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpGet("away-with-make-up-time-entries/{awayWithMakeUpTimeEntryId}")]
    public Task<GetAwayWithMakeUpTimeEntryResponse> GetAwayWithMakeUpTimeEntryAsync(
        [Required][FromRoute] long awayWithMakeUpTimeEntryId,
        [FromServices] GetAwayWithMakeUpTimeEntryHandler getAwayWithMakeUpTimeEntryHandler
    )
    {
        return getAwayWithMakeUpTimeEntryHandler.HandleAsync(awayWithMakeUpTimeEntryId);
    }

    [EndpointSummary("Create an away with make up time entry")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpPost("away-with-make-up-time-entries")]
    public Task<CreateAwayWithMakeUpTimeEntryResponse> CreateAwayWithMakeUpTimeEntryAsync(
        [Required][FromBody] CreateAwayWithMakeUpTimeEntryRequest createAwayWithMakeUpTimeEntryRequest,
        [FromServices] CreateAwayWithMakeUpTimeEntryHandler createAwayWithMakeUpTimeEntryHandler
    )
    {
        return createAwayWithMakeUpTimeEntryHandler.HandleAsync(createAwayWithMakeUpTimeEntryRequest);
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

    [EndpointSummary("Update an away with make up time entry")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpPost("away-with-make-up-time-entries/{awayWithMakeUpTimeEntryId}")]
    public Task UpdateAwayWithMakeUpTimeEntryAsync(
        [Required][FromRoute] long awayWithMakeUpTimeEntryId,
        [Required][FromBody] UpdateAwayWithMakeUpTimeEntryRequest updateAwayWithMakeUpTimeEntryRequest,
        [FromServices] UpdateAwayWithMakeUpTimeEntryHandler updateAwayWithMakeUpTimeEntryHandler
    )
    {
        return updateAwayWithMakeUpTimeEntryHandler.HandleAsync(awayWithMakeUpTimeEntryId, updateAwayWithMakeUpTimeEntryRequest);
    }

    [EndpointSummary("Get a sick leave entry")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpGet("sick-leave-entries/{sickLeaveEntryId}")]
    public Task<GetSickLeaveEntryResponse> GetSickLeaveEntryAsync(
        [Required][FromRoute] long sickLeaveEntryId
    )
    {
        throw new NotImplementedException();
    }

    [EndpointSummary("Create a sick leave entry")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpPost("sick-leave-entries")]
    public Task<CreateSickLeaveEntryResponse> CreateSickLeaveEntryAsync(
        [Required][FromBody] CreateSickLeaveEntryRequest createSickLeaveEntryRequest,
        [FromServices] CreateSickLeaveEntryHandler createSickLeaveEntryHandler
    )
    {
        return createSickLeaveEntryHandler.HandleAsync(createSickLeaveEntryRequest);
    }

    [EndpointSummary("Update a sick leave entry")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpPost("sick-leave-entries/{sickLeaveEntryId}")]
    public Task UpdateSickLeaveEntryAsync(
        [Required][FromRoute] long sickLeaveEntryId,
        [Required][FromBody] UpdateSickLeaveEntryRequest updateSickLeaveEntryRequest
    )
    {
        throw new NotImplementedException();
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
    public Task<object> HardDeleteEntryAsync(
        [Required][FromRoute] long entryId,
        [FromServices] HardDeleteEntryHandler hardDeleteEntryHandler
    )
    {
        return hardDeleteEntryHandler.HandleAsync(entryId);
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
