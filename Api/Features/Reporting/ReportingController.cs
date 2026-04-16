using System.ComponentModel.DataAnnotations;
using Application.Features.Reporting.GetAllEmployees;
using Application.Features.Reporting.GetPersonalReport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Filters;

namespace Api.Features.Reporting;

[Authorize]
[ApiController]
[Route("api/reporting")]
public class ReportingController : ControllerBase
{
    [EndpointSummary("Get all employees")]
    [RequiresPermission(UserClaimsProvider.CanViewPersonalReport)]
    [HttpGet("employees")]
    public Task<GetAllEmployeesResponse> GetAllEmployeesAsync(
       [FromServices] GetAllEmployeesHandler getAllEmployeesHandler
    )
    {
        return getAllEmployeesHandler.HandleAsync();
    }

    [EndpointSummary("Get a personal employee report sorted by date in ascending order")]
    [RequiresPermission(UserClaimsProvider.CanViewPersonalReport)]
    [HttpGet("personal-report")]
    public Task<GetPersonalReportResponse> GetPersonalReportAsync(
        [Required][FromQuery] long employeeId,
        [Required][FromQuery] int year,
        [Required][FromQuery] int month,
        [FromServices] GetPersonalReportHandler getPersonalReportHandler
    )
    {
        return getPersonalReportHandler.HandleAsync(employeeId, year, month);
    }
}
