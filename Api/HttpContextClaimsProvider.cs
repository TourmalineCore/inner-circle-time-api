using System.Security.Claims;
using Application;

namespace Api;

public class HttpContextClaimsProvider : IClaimsProvider
{
    private const string TenantIdClaimType = "tenantId";
    private const string EmployeeIdIdClaimType = "employeeId";

    private long _tenantId;
    private long _employeeId;

    public HttpContextClaimsProvider(IHttpContextAccessor httpContext)
    {
        _tenantId = long.Parse(httpContext.HttpContext!.User.FindFirstValue(TenantIdClaimType)!);
        _employeeId = long.Parse(httpContext.HttpContext!.User.FindFirstValue(EmployeeIdIdClaimType)!);
    }

    public long TenantId
    {
        get
        {
            return _tenantId;
        }
    }

    public long EmployeeId
    {
        get
        {
            return _employeeId;
        }
    }
}
