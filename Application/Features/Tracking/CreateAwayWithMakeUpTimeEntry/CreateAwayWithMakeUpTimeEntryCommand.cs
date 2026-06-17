namespace Application.Features.Tracking.CreateAwayWithMakeUpTimeEntry;

public class CreateAwayWithMakeUpTimeEntryCommand : DbValidationEntryCommandBase<CreateAwayWithMakeUpTimeEntryRequest>
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public CreateAwayWithMakeUpTimeEntryCommand(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public async Task<long> ExecuteAsync(CreateAwayWithMakeUpTimeEntryRequest createAwayWithMakeUpTimeEntryRequest)
    {
        return await MakeChangesInDbAsync(createAwayWithMakeUpTimeEntryRequest);
    }

    protected override async Task<long> MakeChangesToEntryAsync(CreateAwayWithMakeUpTimeEntryRequest createAwayWithMakeUpTimeEntryRequest)
    {
        var awayWithMakeUpTimeEntry = new AwayWithMakeUpTimeEntry
        {
            TenantId = _claimsProvider.TenantId,
            EmployeeId = _claimsProvider.EmployeeId,
            StartTime = createAwayWithMakeUpTimeEntryRequest.StartTime,
            EndTime = createAwayWithMakeUpTimeEntryRequest.EndTime,
            Description = createAwayWithMakeUpTimeEntryRequest.Description,
            MakeUpTimeList = createAwayWithMakeUpTimeEntryRequest
                .MakeUpTimeList
                .Select(x => new MakeUpTimeEntry
                {
                    TenantId = _claimsProvider.TenantId,
                    EmployeeId = _claimsProvider.EmployeeId,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime
                }).ToList()
        };

        await _context
            .AwayWithMakeUpTimeEntries
            .AddAsync(awayWithMakeUpTimeEntry);

        await _context.SaveChangesAsync();

        return awayWithMakeUpTimeEntry.Id;
    }
}
