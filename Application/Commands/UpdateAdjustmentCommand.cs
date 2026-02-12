using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class UpdateAdjustmentCommandParams
{
    public required long Id { get; set; }

    public required EventType Type { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }
}

public class UpdateAdjustmentCommand
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public UpdateAdjustmentCommand(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public async Task<long> ExecuteAsync(UpdateAdjustmentCommandParams updateAdjustmentCommandParams)
    {
        await _context
            .QueryableWithinTenant<Adjustment>()
            .Where(x => x.EmployeeId == _claimsProvider.EmployeeId)
            .Where(x => x.Id == updateAdjustmentCommandParams.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.Type, updateAdjustmentCommandParams.Type)
                .SetProperty(x => x.StartTime, updateAdjustmentCommandParams.StartTime)
                .SetProperty(x => x.EndTime, updateAdjustmentCommandParams.EndTime)
            );

        return updateAdjustmentCommandParams.Id;
    }

}
