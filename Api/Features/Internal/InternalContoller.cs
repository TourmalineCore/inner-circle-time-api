using System.ComponentModel.DataAnnotations;
using Application.ExternalDeps.AssignmentsApi;
using Application.Features.Internal.GetEmployeesEntriesByProjectAndPeriod;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Internal;

[ApiController]
[Route("internal")]
public class InternalController : ControllerBase
{
    [EndpointSummary("Get employees time entries by project")]
    [HttpGet("employees-entries-by-project")]
    public Task<GetEmployeesEntriesByProjectAndPeriodResponse> GetEmployeesEntriesByProjectAndPeriod(
        [Required][FromQuery] long projectId,
        [Required][FromQuery] DateOnly startDate,
        [Required][FromQuery] DateOnly endDate,
        [Required][FromQuery] long tenantId,
        [FromServices] GetEmployeesEntriesByProjectAndPeriodHandler getEmployeesEntriesByProjectAndPeriod
    )
    {
        return getEmployeesEntriesByProjectAndPeriod.HandleAsync(projectId, startDate, endDate, tenantId);
    }

    [EndpointSummary("Get all projects")]
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
