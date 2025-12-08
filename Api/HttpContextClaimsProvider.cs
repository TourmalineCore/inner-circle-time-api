using System.Security.Claims;
using Application;

namespace Api;

public class HttpContextClaimsProvider : IClaimsProvider
{
    private const string TenantIdClaimType = "tenantId";
    private const string EmployeeIdClaimType = "employeeId";

    private long _tenantId;
    private long _employeeId;

    public HttpContextClaimsProvider(IHttpContextAccessor httpContext)
    {
        _tenantId = long.Parse(httpContext.HttpContext!.User.FindFirstValue(TenantIdClaimType)!);
        _employeeId = long.Parse(httpContext.HttpContext!.User.FindFirstValue(EmployeeIdClaimType)!);
    }

    public long TenantId => _tenantId;

    public long EmployeeId => _employeeId;
}
