using System.Security.Claims;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;

namespace Api;

public class UserClaimsProvider : IUserClaimsProvider
{
    public const string PermissionClaimType = "permissions";

    public const string AUTO_TESTS_ONLY_IsWorkEntriesHardDeleteAllowed = "AUTO_TESTS_ONLY_IsWorkEntriesHardDeleteAllowed";
    public const string CanManagePersonalTimeTracker = "CanManagePersonalTimeTracker";

    public Task<List<Claim>> GetUserClaimsAsync(string login)
    {
        throw new NotImplementedException();
    }
}
