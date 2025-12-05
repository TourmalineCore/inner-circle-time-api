using System.ComponentModel.DataAnnotations;
using Api.Features.Tracking.CreateWorkEntry;
using Api.Features.Tracking.GetWorkEntriesByPeriod;
using Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using TourmalineCore.AspNetCore.JwtAuthentication.Core.Filters;

namespace Api.Features.Tracking;

[Authorize]
[ApiController]
[Route("api/time/tracking")]
public class TrackingController : ControllerBase
{
    [EndpointSummary("Get work entries by period")]
    // [RequiresPermission(UserClaimsProvider.CanViewTracker)]
    [HttpGet("work-entries")]
    public Task<GetWorkEntriesByPeriodResponse> GetWorkEntriesByPeriodAsync(
        [Required][FromQuery] string startTime,
        [Required][FromQuery] string endTime,
        [FromServices] GetWorkEntriesByPeriodHandler getWorkEntriesByPeriodHandler,
        [FromServices] IClaimsProvider claimsProvider
    )
    {
        return getWorkEntriesByPeriodHandler.HandleAsync(startTime, endTime, claimsProvider.EmployeeId);
    }

    [EndpointSummary("Create a work entry")]
    // [RequiresPermission(UserClaimsProvider.CanViewTracker)]
    [HttpPost("work-entries")]
    public Task<CreateWorkEntryResponse> CreateWorkEntryAsync(
        [FromServices] CreateWorkEntryHandler createWorkEntryHandler,
        [Required][FromBody] CreateWorkEntryRequest createWorkEntryRequest,
        [FromServices] IClaimsProvider claimsProvider
    )
    {
        return createWorkEntryHandler.HandleAsync(createWorkEntryRequest, claimsProvider.EmployeeId);
    }
}
