using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tracking.UpdateVacationEntry;

public class UpdateVacationEntryCommand : DbValidationEntryCommandBase<UpdateVacationEntryRequest>
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public UpdateVacationEntryCommand(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public async Task<long> ExecuteAsync(UpdateVacationEntryRequest updateVacationEntryRequest)
    {
        return await MakeChangesInDbAsync(updateVacationEntryRequest);
    }

    protected override async Task<long> MakeChangesToEntryAsync(UpdateVacationEntryRequest updateVacationEntryRequest)
    {
        var duration = PeriodToDurationConverter.ConvertToDuration(new Period
        {
            StartDate = updateVacationEntryRequest.Period.StartDate,
            EndDate = updateVacationEntryRequest.Period.EndDate,
        });

        await _context
            .QueryableWithinTenant<VacationEntry>()
            .Where(x => x.EmployeeId == _claimsProvider.EmployeeId)
            .Where(x => x.Id == updateVacationEntryRequest.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.StartTime, duration.StartTime)
                .SetProperty(x => x.EndTime, duration.EndTime)
                .SetProperty(x => x.IsUnpaid, updateVacationEntryRequest.IsUnpaid)
            );

        return updateVacationEntryRequest.Id;
    }
}
