using System.ComponentModel.DataAnnotations;
using Application.ExternalDeps.AssignmentsApi;
using Application.Features.Internal.GetAllProjects;
using Application.Features.Internal.GetEmployeesTrackedTaskHours;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Filters;

namespace Api.Features.Internal;

[Authorize]
[ApiController]
[Route("/api/internal")]
public class InternalController : ControllerBase
{
    [EndpointSummary("Get employees tracked task hours")]
    [RequiresPermission(UserClaimsProvider.CanViewAllTimeTrackerEntries)]
    [HttpGet("projects/tracked-task-hours")]
    public Task<GetEmployeesTrackedTaskHoursResponse> GetEmployeesTrackedTaskHoursAsync(
        [Required][FromQuery] long projectId,
        [Required][FromQuery] DateOnly startDate,
        [Required][FromQuery] DateOnly endDate,
        [FromServices] GetEmployeesTrackedTaskHoursHandler getEmployeesTrackedTaskHoursHandler
    )
    {
        return getEmployeesTrackedTaskHoursHandler.HandleAsync(projectId, startDate, endDate);
    }

    [EndpointSummary("Get all projects")]
    [RequiresPermission(UserClaimsProvider.CanViewAllProjects)]
    [HttpGet("projects")]
    public Task<ProjectsResponse> GetAllProjectsAsync(
        [FromServices] GetAllProjectsHandler getAllProjectsHandler
    )
    {
        return getAllProjectsHandler.HandleAsync();
    }
}
