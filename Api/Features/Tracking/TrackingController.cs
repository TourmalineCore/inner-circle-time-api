using System.ComponentModel.DataAnnotations;
using Api.Features.Tracking.CreateTaskEntry;
using Api.Features.Tracking.CreateUnwellEntry;
using Api.Features.Tracking.GetEntriesByPeriod;
using Api.Features.Tracking.UpdateTaskEntry;
using Api.Features.Tracking.UpdateUnwellEntry;
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
    // Todo: delete after UI change contract to /entries
    [EndpointSummary("Get work entries by period")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpGet("work-entries")]
    public Task<GetEntriesByPeriodResponse> GetWorkEntriesByPeriodAsync(
        [Required][FromQuery] DateOnly startDate,
        [Required][FromQuery] DateOnly endDate,
        [FromServices] GetEntriesByPeriodHandler getEntriesByPeriodHandler
    )
    {
        return getEntriesByPeriodHandler.HandleAsync(startDate, endDate);
    }

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

    // Todo: delete after UI change contract to /task-entries
    [EndpointSummary("Create a work entry")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpPost("work-entries")]
    public Task<CreateTaskEntryResponse> CreateWorkEntryAsync(
        [Required][FromBody] CreateTaskEntryRequest createTaskEntryRequest,
        [FromServices] CreateTaskEntryHandler createTaskEntryHandler
    )
    {
        return createTaskEntryHandler.HandleAsync(createTaskEntryRequest);
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

    // Todo: delete after UI change contract to /task-entries
    [EndpointSummary("Update a work entry")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpPost("work-entries/{workEntryId}")]
    public Task UpdateWorkEntryAsync(
        [Required][FromRoute] long workEntryId,
        [Required][FromBody] UpdateTaskEntryRequest updateTaskEntryRequest,
        [FromServices] UpdateTaskEntryHandler updateTaskEntryHandler
    )
    {
        return updateTaskEntryHandler.HandleAsync(workEntryId, updateTaskEntryRequest);
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

    // Todo: delete after UI change contract to /task-entries
    [EndpointSummary("Get employee projects by period")]
    [RequiresPermission(UserClaimsProvider.CanManagePersonalTimeTracker)]
    [HttpGet("work-entries/projects")]
    public async Task<ProjectsResponse> GetEmployeeProjectsByPeriodToWorkEntriesAsync(
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

    // Todo: delete after UI change contract to /entries
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
            isDeleted = await hardDeleteEntityCommand.ExecuteAsync<TrackedEntryBase>(workEntryId)
        };
    }

    [EndpointSummary("Deletes specific entry")]
    [RequiresPermission(UserClaimsProvider.AUTO_TESTS_ONLY_IsEntriesHardDeleteAllowed)]
    [HttpDelete("entries/{entryId}/hard-delete")]
    public async Task<object> HardDeleteEntryAsync(
    [Required][FromRoute] long entryId,
    [FromServices] HardDeleteEntityCommand hardDeleteEntityCommand
    )
    {
        return new
        {
            isDeleted = await hardDeleteEntityCommand.ExecuteAsync<TrackedEntryBase>(entryId)
        };
    }
}
