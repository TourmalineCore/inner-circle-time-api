using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class UpdateUnwellEntryCommandParams
{
    public required long Id { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }
}

public class UpdateUnwellEntryCommand : DbValidationWorkEntryCommandBase<UpdateUnwellEntryCommandParams>
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

    public async Task<long> ExecuteAsync(UpdateUnwellEntryCommandParams updateUnwellEntryCommandParams)
    {
        return await MakeChangesInDbAsync(updateUnwellEntryCommandParams);
    }

    protected override async Task<long> MakeChangesToWorkEntryAsync(UpdateUnwellEntryCommandParams commandParams)
    {
        await _context
            .QueryableWithinTenant<UnwellEntry>()
            .Where(x => x.EmployeeId == _claimsProvider.EmployeeId)
            .Where(x => x.Id == commandParams.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.StartTime, commandParams.StartTime)
                .SetProperty(x => x.EndTime, commandParams.EndTime)
            );

        return commandParams.Id;
    }
}
