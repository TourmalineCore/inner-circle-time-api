using Microsoft.EntityFrameworkCore;

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

        var makeUpTimeListToDelete = await _context
            .QueryableWithinTenant<MakeUpTimeEntry>()
            .Where(x => x.RelatedEntryId == updateAwayWithMakeUpTimeEntryRequest.Id)
            .ToListAsync();

        _context.RemoveRange(makeUpTimeListToDelete);

        var newMakeUpTimeList = updateAwayWithMakeUpTimeEntryRequest
           .MakeUpTimeList
           .Select(x => new MakeUpTimeEntry
           {
               TenantId = _claimsProvider.TenantId,
               EmployeeId = _claimsProvider.EmployeeId,
               RelatedEntryId = updateAwayWithMakeUpTimeEntryRequest.Id,
               StartTime = x.StartTime,
               EndTime = x.EndTime
           }).ToList();

        await _context
            .MakeUpTimeEntries
            .AddRangeAsync(newMakeUpTimeList);

        await _context.SaveChangesAsync();

        return updateAwayWithMakeUpTimeEntryRequest.Id;
    }
}
