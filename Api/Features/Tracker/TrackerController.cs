using System.ComponentModel.DataAnnotations;
using Api.Features.Tracker.CreateWorkEntry;
using Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using TourmalineCore.AspNetCore.JwtAuthentication.Core.Filters;

namespace Api.Features.Tracker;

[Authorize]
[ApiController]
[Route("api/time/tracker")]
public class TrackerController : ControllerBase
{
    // [RequiresPermission(UserClaimsProvider.CanViewTracker)]
    [HttpPost("work-entries")]
    [EndpointSummary("Create a work entry")]
    public Task<CreateWorkEntryResponse> CreateWorkEntryAsync(
        [FromServices] CreateWorkEntryHandler createWorkEntryHandler,
        [Required][FromBody] CreateWorkEntryRequest createWorkEntryRequest,
        [FromServices] IClaimsProvider claimsProvider
    )
    {
        return createWorkEntryHandler.HandleAsync(createWorkEntryRequest, claimsProvider.EmployeeId);
    }
}
