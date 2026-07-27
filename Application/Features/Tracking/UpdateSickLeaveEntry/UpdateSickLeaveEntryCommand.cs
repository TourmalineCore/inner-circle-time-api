using Core;
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
        var duration = PeriodToDurationConverter.ConvertToDuration(new Period
        {
            StartDate = updateSickLeaveEntryRequest.Period.StartDate,
            EndDate = updateSickLeaveEntryRequest.Period.EndDate,
        });

        await _context
            .QueryableWithinTenant<SickLeaveEntry>()
            .Where(x => x.EmployeeId == _claimsProvider.EmployeeId)
            .Where(x => x.Id == updateSickLeaveEntryRequest.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.StartTime, duration.StartTime)
                .SetProperty(x => x.EndTime, duration.EndTime)
            );

        return updateSickLeaveEntryRequest.Id;
    }
}
