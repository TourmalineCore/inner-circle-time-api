using Core.Entities;
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
        await _context
            .QueryableWithinTenant<awayWithMakeUpTimeEntry>()
            .Where(x => x.EmployeeId == _claimsProvider.EmployeeId)
            .Where(x => x.Id == updateAwayWithMakeUpTimeEntryRequest.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.StartTime, updateAwayWithMakeUpTimeEntryRequest.StartTime)
                .SetProperty(x => x.EndTime, updateAwayWithMakeUpTimeEntryRequest.EndTime)
                .SetProperty(x => x.Description, updateAwayWithMakeUpTimeEntryRequest.Description)
            );

        // awayWithMakeUpTimeEntry.MakeUpTimeList = createAwayWithMakeUpTimeEntryRequest
        //     .MakeUpTimeList
        //     .Select(
        //         x => new MakeUpTimeEntry
        //         {
        //             RelatedEntryId = awayWithMakeUpTimeEntry.Id,
        //             StartTime = x.StartTime,
        //             EndTime = x.EndTime
        //         }
        //     ).ToList();
            
        await _context.Set<MakeUpTimeEntry>() 
            .Where(x => x.RelatedEntryId == updateAwayWithMakeUpTimeEntryRequest.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x=> x.StartTime, updateAwayWithMakeUpTimeEntryRequest.StartTime)
                .SetProperty(x=> x.EndTime, updateAwayWithMakeUpTimeEntryRequest.EndTime)
            );

        return updateAwayWithMakeUpTimeEntryRequest.Id;
    }
}
