using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tracking.UpdateUnwellEntry;

public class UpdateUnwellEntryCommand : DbValidationEntryCommandBase<UpdateUnwellEntryRequest>
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public UpdateUnwellEntryCommand(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public async Task<long> ExecuteAsync(UpdateUnwellEntryRequest updateUnwellEntryRequest)
    {
        return await MakeChangesInDbAsync(updateUnwellEntryRequest);
    }

    protected override async Task<long> MakeChangesToEntryAsync(UpdateUnwellEntryRequest updateUnwellEntryRequest)
    {
        await _context
            .QueryableWithinTenant<UnwellEntry>()
            .Where(x => x.EmployeeId == _claimsProvider.EmployeeId)
            .Where(x => x.Id == updateUnwellEntryRequest.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.StartTime, updateUnwellEntryRequest.StartTime)
                .SetProperty(x => x.EndTime, updateUnwellEntryRequest.EndTime)
            );

        return updateUnwellEntryRequest.Id;
    }
}
