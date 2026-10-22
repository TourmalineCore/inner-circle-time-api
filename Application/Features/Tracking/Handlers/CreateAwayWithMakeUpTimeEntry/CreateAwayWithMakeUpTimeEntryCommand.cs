using Application.Features.Tracking.Validators;
using Core.Features.Tracking.Entities;

namespace Application.Features.Tracking.Handlers.CreateAwayWithMakeUpTimeEntry;

public class CreateAwayWithMakeUpTimeEntryCommand : DbValidationEntryCommandBase<CreateAwayWithMakeUpTimeEntryRequest>
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public CreateAwayWithMakeUpTimeEntryCommand(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    : base(context)
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
                    RelatedEntryType = EntryType.AwayWithMakeUpTime,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime
                })
                .ToList()
        };

        await _context
            .AwayWithMakeUpTimeEntries
            .AddAsync(awayWithMakeUpTimeEntry);

        await _context.SaveChangesAsync();

        return awayWithMakeUpTimeEntry.Id;
    }

    protected override async Task<IReadOnlyList<EntryDurationToCheck>> GetEntryDurationsToCheckAsync(CreateAwayWithMakeUpTimeEntryRequest createAwayWithMakeUpTimeEntryRequest)
    {
        var contexts = new List<EntryDurationToCheck>
        {
            new EntryDurationToCheck
            {

                EmployeeId = _claimsProvider.EmployeeId,
                StartTime = createAwayWithMakeUpTimeEntryRequest.StartTime,
                EndTime = createAwayWithMakeUpTimeEntryRequest.EndTime,
                EntryType = EntryType.AwayWithMakeUpTime
            }
        };

        foreach (var makeUpTime in createAwayWithMakeUpTimeEntryRequest.MakeUpTimeList)
        {
            contexts.Add(new EntryDurationToCheck
            {

                EmployeeId = _claimsProvider.EmployeeId,
                StartTime = makeUpTime.StartTime,
                EndTime = makeUpTime.EndTime,
                EntryType = EntryType.MakeUpTime
            });
        }

        return contexts;
    }
}
