using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Application.Features.Tracking.UpdateAwayWithMakeUpTimeEntry;

public class UpdateAwayWithMakeUpTimeEntryCommand : DbValidationEntryCommandBase<UpdateAwayWithMakeUpTimeEntryRequest>
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public UpdateAwayWithMakeUpTimeEntryCommand(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public async Task<long> ExecuteAsync(UpdateAwayWithMakeUpTimeEntryRequest updateAwayWithMakeUpTimeEntryRequest)
    {
        return await MakeChangesInDbAsync(updateAwayWithMakeUpTimeEntryRequest);
    }

    protected override async Task<long> MakeChangesToEntryAsync(UpdateAwayWithMakeUpTimeEntryRequest updateAwayWithMakeUpTimeEntryRequest)
    {
        var awayWithMakeUpTimeEntry = await _context
            .QueryableWithinTenant<AwayWithMakeUpTimeEntry>()
            .Where(x => x.EmployeeId == _claimsProvider.EmployeeId)
            .Where(x => x.Id == updateAwayWithMakeUpTimeEntryRequest.Id)
            .FirstOrDefaultAsync();

        if (awayWithMakeUpTimeEntry == null)
        {
            throw new ArgumentException($"Away With Make Up Time Entry with id {updateAwayWithMakeUpTimeEntryRequest.Id} does not exist");
        }

        awayWithMakeUpTimeEntry.Description = updateAwayWithMakeUpTimeEntryRequest.Description;
        awayWithMakeUpTimeEntry.StartTime = updateAwayWithMakeUpTimeEntryRequest.StartTime;
        awayWithMakeUpTimeEntry.EndTime = updateAwayWithMakeUpTimeEntryRequest.EndTime;

        var makeUpTimeListFromDb = await _context
            .QueryableWithinTenant<MakeUpTimeEntry>()
            .Where(x => x.RelatedEntryId == updateAwayWithMakeUpTimeEntryRequest.Id)
            .ToListAsync();

        foreach (var makeUpTimeFromDb in makeUpTimeListFromDb)
        {
            var doesNotEntryWithSameTimeExistInRequest = !updateAwayWithMakeUpTimeEntryRequest
                .MakeUpTimeList
                .Any(x => x.StartTime == makeUpTimeFromDb.StartTime && x.EndTime == makeUpTimeFromDb.EndTime);

            if (doesNotEntryWithSameTimeExistInRequest)
            {
                _context
                    .MakeUpTimeEntries
                    .Remove(makeUpTimeFromDb);
            }
        }

        foreach (var makeUpTimeFromRequest in updateAwayWithMakeUpTimeEntryRequest.MakeUpTimeList)
        {
            var doesNotEntryWithSameTimeExistInDb = !makeUpTimeListFromDb
                .Any(x => x.StartTime == makeUpTimeFromRequest.StartTime && x.EndTime == makeUpTimeFromRequest.EndTime);

            if (doesNotEntryWithSameTimeExistInDb)
            {
                var makeUpTimeEntry = new MakeUpTimeEntry
                {
                    TenantId = _claimsProvider.TenantId,
                    EmployeeId = _claimsProvider.EmployeeId,
                    RelatedEntryId = updateAwayWithMakeUpTimeEntryRequest.Id,
                    RelatedEntryType = EntryType.AwayWithMakeUpTime,
                    StartTime = makeUpTimeFromRequest.StartTime,
                    EndTime = makeUpTimeFromRequest.EndTime
                };

                await _context
                    .MakeUpTimeEntries
                    .AddAsync(makeUpTimeEntry);
            }
        }

        await _context.SaveChangesAsync();

        return updateAwayWithMakeUpTimeEntryRequest.Id;
    }
}
