using Core;
using Core.Entities;

namespace Application.Features.Tracking.CreateSickLeaveEntry;

public class CreateSickLeaveEntryCommand : DbValidationEntryCommandBase<CreateSickLeaveEntryRequest>
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public CreateSickLeaveEntryCommand(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public async Task<long> ExecuteAsync(CreateSickLeaveEntryRequest createSickLeaveEntryRequest)
    {
        return await MakeChangesInDbAsync(createSickLeaveEntryRequest);
    }

    protected override async Task<long> MakeChangesToEntryAsync(CreateSickLeaveEntryRequest createSickLeaveEntryRequest)
    {
        var duration = new PeriodToDurationConverter().ConvertToDuration(new Period
        {
            StartDate = createSickLeaveEntryRequest.Period.StartDate,
            EndDate = createSickLeaveEntryRequest.Period.EndDate,
        });

        var sickLeaveEntry = new SickLeaveEntry
        {
            TenantId = _claimsProvider.TenantId,
            EmployeeId = _claimsProvider.EmployeeId,
            StartTime = duration.StartTime,
            EndTime = duration.EndTime,
        };

        await _context
            .SickLeaveEntries
            .AddAsync(sickLeaveEntry);

        await _context.SaveChangesAsync();

        return sickLeaveEntry.Id;
    }
}
