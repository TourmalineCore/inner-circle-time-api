using System.ComponentModel.DataAnnotations;
using Application.ExternalDeps.AssignmentsApi;
using Application.Features.Internal.GetEmployeesTrackedTaskHoursByProject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Filters;

namespace Api.Features.Internal;

[Authorize]
[ApiController]
[Route("/api/internal")]
public class InternalController : ControllerBase
{
    [EndpointSummary("Get employees time entries by project")]
    [RequiresPermission(UserClaimsProvider.CanViewAllTimeTrackerEntries)]
    [HttpGet("projects/tracked-task-hours")]
    public Task<GetEmployeesTrackedTaskHoursByProjectResponse> GetEmployeesEntriesByProjectAndPeriod(
        [Required][FromQuery] long projectId,
        [Required][FromQuery] DateOnly startDate,
        [Required][FromQuery] DateOnly endDate,
        [FromServices] GetEmployeesTrackedTaskHoursByProjectHandler getEmployeesEntriesByProjectAndPeriod
    )
    {
        return getEmployeesEntriesByProjectAndPeriod.HandleAsync(projectId, startDate, endDate);
    }

    [EndpointSummary("Get all projects")]
    [RequiresPermission(UserClaimsProvider.CanViewAllProjects)]
    [HttpGet("projects")]
    public async Task<ProjectsResponse> GetAllProjectsAsync(
        [FromServices] IAssignmentsApi assignmentsApi
    )
    {
        return new ProjectsResponse
        {
            Projects = await assignmentsApi.GetAllProjectsAsync()
        };
    }
}
