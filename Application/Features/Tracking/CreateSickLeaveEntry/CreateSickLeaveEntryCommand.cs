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
        var startTime = createSickLeaveEntryRequest
            .Period
            .StartDate
            .ToDateTime(TimeOnly.MinValue);

        var endTime = createSickLeaveEntryRequest
            .Period
            .EndDate
            .AddDays(1)
            .ToDateTime(TimeOnly.MinValue);

        var sickLeaveEntry = new SickLeaveEntry
        {
            TenantId = _claimsProvider.TenantId,
            EmployeeId = _claimsProvider.EmployeeId,
            StartTime = startTime,
            EndTime = endTime,
        };

        await _context
            .SickLeaveEntries
            .AddAsync(sickLeaveEntry);

        await _context.SaveChangesAsync();

        return sickLeaveEntry.Id;
    }
}
