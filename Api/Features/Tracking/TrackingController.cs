using System.ComponentModel.DataAnnotations;
using Api.Features.Tracking.CreateWorkEntry;
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
