using Core.Entities;

namespace Application.Commands;

public class CreateUnwellEntryCommandParams
{
    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }
}

public class CreateUnwellEntryCommand : DbValidationWorkEntryCommandBase<CreateUnwellEntryCommandParams>
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

    public async Task<long> ExecuteAsync(CreateUnwellEntryCommandParams createUnwellEntryCommandParams)
    {
        return await MakeChangesInDbAsync(createUnwellEntryCommandParams);
    }

    protected override async Task<long> MakeChangesToWorkEntryAsync(CreateUnwellEntryCommandParams commandParams)
    {
        var unwellEntry = new UnwellEntry
        {
            TenantId = _claimsProvider.TenantId,
            EmployeeId = _claimsProvider.EmployeeId,
            StartTime = commandParams.StartTime,
            EndTime = commandParams.EndTime,
        };

        await _context
            .UnwellEntries
            .AddAsync(unwellEntry);

        await _context.SaveChangesAsync();

        return unwellEntry.Id;
    }
}
