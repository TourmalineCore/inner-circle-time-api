using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tracking.UpdateSickLeaveEntry;

public class UpdateSickLeaveEntryCommand : DbValidationEntryCommandBase<UpdateSickLeaveEntryRequest>
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public UpdateSickLeaveEntryCommand(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public async Task<long> ExecuteAsync(UpdateSickLeaveEntryRequest updateSickLeaveEntryRequest)
    {
        return await MakeChangesInDbAsync(updateSickLeaveEntryRequest);
    }

    protected override async Task<long> MakeChangesToEntryAsync(UpdateSickLeaveEntryRequest updateSickLeaveEntryRequest)
    {
        var startTime = updateSickLeaveEntryRequest
            .Period
            .StartDate
            .ToDateTime(TimeOnly.MinValue);

        var endTime = updateSickLeaveEntryRequest
            .Period
            .EndDate
            .AddDays(1)
            .ToDateTime(TimeOnly.MinValue);

        await _context
            .QueryableWithinTenant<SickLeaveEntry>()
            .Where(x => x.EmployeeId == _claimsProvider.EmployeeId)
            .Where(x => x.Id == updateSickLeaveEntryRequest.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.StartTime, startTime)
                .SetProperty(x => x.EndTime, endTime)
            );

        return updateSickLeaveEntryRequest.Id;
    }
}
