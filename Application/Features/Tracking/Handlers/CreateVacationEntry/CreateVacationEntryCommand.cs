using Application.Features.Tracking.Validators;
using Core;
using Core.Features.Tracking;
using Core.Features.Tracking.Entities;

namespace Application.Features.Tracking.Handlers.CreateVacationEntry;

public class CreateVacationEntryCommand : DbValidationEntryCommandBase<CreateVacationEntryRequest>
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public CreateVacationEntryCommand(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    : base(context)
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public async Task<long> ExecuteAsync(CreateVacationEntryRequest createVacationEntryRequest)
    {
        return await MakeChangesInDbAsync(createVacationEntryRequest);
    }

    protected override async Task<long> MakeChangesToEntryAsync(CreateVacationEntryRequest createVacationEntryRequest)
    {
        var duration = PeriodToDurationConverter.ConvertToDuration(new Period
        {
            StartDate = createVacationEntryRequest.Period.StartDate,
            EndDate = createVacationEntryRequest.Period.EndDate,
        });

        var vacationEntry = new VacationEntry
        {
            TenantId = _claimsProvider.TenantId,
            EmployeeId = _claimsProvider.EmployeeId,
            StartTime = duration.StartTime,
            EndTime = duration.EndTime,
            IsUnpaid = createVacationEntryRequest.IsUnpaid
        };

        await _context
            .VacationEntries
            .AddAsync(vacationEntry);

        await _context.SaveChangesAsync();

        return vacationEntry.Id;
    }

    protected override async Task<IReadOnlyList<EntryDurationToCheck>> GetEntryDurationsToCheckAsync(CreateVacationEntryRequest createVacationEntryRequest)
    {
        var duration = PeriodToDurationConverter.ConvertToDuration(new Period
        {
            StartDate = createVacationEntryRequest.Period.StartDate,
            EndDate = createVacationEntryRequest.Period.EndDate,
        });

        return new List<EntryDurationToCheck>
        {
            new EntryDurationToCheck
            {
                EmployeeId = _claimsProvider.EmployeeId,
                StartTime = duration.StartTime,
                EndTime = duration.EndTime,
                EntryType = EntryType.Vacation
            }
        };
    }
}
