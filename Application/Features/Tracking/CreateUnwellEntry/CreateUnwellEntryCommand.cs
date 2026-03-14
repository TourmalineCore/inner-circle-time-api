using Core.Entities;

namespace Application.Features.Tracking.CreateUnwellEntry;

public class CreateUnwellEntryCommand : DbValidationEntryCommandBase<CreateUnwellEntryTestRequest>
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public CreateUnwellEntryCommand(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public async Task<long> ExecuteAsync(CreateUnwellEntryTestRequest createUnwellEntryRequest)
    {
        return await MakeChangesInDbAsync(createUnwellEntryRequest);
    }

    protected override async Task<long> MakeChangesToEntryAsync(CreateUnwellEntryTestRequest createUnwellEntryRequest)
    {
        var unwellEntry = new UnwellEntry
        {
            TenantId = _claimsProvider.TenantId,
            EmployeeId = _claimsProvider.EmployeeId,
            StartTime = createUnwellEntryRequest.StartTime,
            EndTime = createUnwellEntryRequest.EndTime,
        };

        await _context
            .UnwellEntries
            .AddAsync(unwellEntry);

        await _context.SaveChangesAsync();

        return unwellEntry.Id;
    }
}
