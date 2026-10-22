using Core;
using Core.Features.Tracking;
using Core.Features.Tracking.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tracking.Handlers.UpdateSickLeaveEntry;

public class UpdateSickLeaveEntryCommand : DbValidationEntryCommandBase<UpdateSickLeaveEntryRequest>
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public UpdateSickLeaveEntryCommand(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    : base(context)
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

    protected override async Task<IReadOnlyList<EntryDurationToCheck>> GetEntryDurationsToCheckAsync(UpdateSickLeaveEntryRequest updateSickLeaveEntryRequest)
    {
        var duration = PeriodToDurationConverter.ConvertToDuration(new Period
        {
            StartDate = updateSickLeaveEntryRequest.Period.StartDate,
            EndDate = updateSickLeaveEntryRequest.Period.EndDate,
        });

        return new List<EntryDurationToCheck>
        {
            new EntryDurationToCheck
            {
                EmployeeId = _claimsProvider.EmployeeId,
                StartTime = duration.StartTime,
                EndTime = duration.EndTime,
                EntryType = EntryType.SickLeave,
                ExcludeEntryIds = [updateSickLeaveEntryRequest.Id],
            }
        };
    }
}
