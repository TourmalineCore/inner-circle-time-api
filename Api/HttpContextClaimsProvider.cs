using System.Security.Claims;
using Application;

namespace Api;

public class HttpContextClaimsProvider : IClaimsProvider
{
    private const string TenantIdClaimType = "tenantId";

    private long _tenantId;

    public HttpContextClaimsProvider(IHttpContextAccessor httpContext)
    {
        _tenantId = long.Parse(httpContext.HttpContext!.User.FindFirstValue(TenantIdClaimType)!);
    }

    public long TenantId
    {
        get
        {
            return _tenantId;
        }
    }
}
