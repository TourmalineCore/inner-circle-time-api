using Core.Entities;

namespace Application.Commands;

public class CreateAdjustmentCommandParams
{
    public required EventType Type { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }
}

public class CreateAdjustmentCommand
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public CreateAdjustmentCommand(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public async Task<long> ExecuteAsync(CreateAdjustmentCommandParams createAdjustmentCommandParams)
    {
        var adjustment = new Adjustment
        {
            TenantId = _claimsProvider.TenantId,
            EmployeeId = _claimsProvider.EmployeeId,
            Type = createAdjustmentCommandParams.Type,
            StartTime = createAdjustmentCommandParams.StartTime,
            EndTime = createAdjustmentCommandParams.EndTime,
        };

        await _context
            .Adjustments
            .AddAsync(adjustment);

        await _context.SaveChangesAsync();

        return adjustment.Id;
    }
}
