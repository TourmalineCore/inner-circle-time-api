using System.ComponentModel.DataAnnotations;
using Application.Features.AvailabilityNotifications.GetAvailabilityNotifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Filters;

namespace Api.Features.AvailabilityNotifications;

[Authorize]
[ApiController]
[Route("availability-notifications")]
public class AvailabilityNotificationsController : ControllerBase
{
    [EndpointSummary("Get availability notifications by period")]
    [RequiresPermission(UserClaimsProvider.CanViewAvailabilityNotifications)]
    [HttpGet("")]
    public Task<GetAvailabilityNotificationsResponse> GetAvailabilityNotificationsAsync(
        [Required][FromQuery] DateOnly startDate,
        [Required][FromQuery] DateOnly endDate
    )
    {
        throw new NotImplementedException();
    }
}
