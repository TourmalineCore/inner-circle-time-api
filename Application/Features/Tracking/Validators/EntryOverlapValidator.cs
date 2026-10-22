using Application.Exceptions;
using Core.Features.Tracking.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tracking.Validators;

public class EntryOverlapValidator
{
    private readonly TenantAppDbContext _context;

    public EntryOverlapValidator(TenantAppDbContext context)
    {
        _context = context;
    }

    public async Task ValidateOverlapsAsync(IReadOnlyList<EntryDurationToCheck> durationsToCheck)
    {
        foreach (var duration in durationsToCheck)
        {
            var overlappedEntry = await _context
                .QueryableWithinTenantAsNoTracking<TrackedEntryBase>()
                .Where(x => x.EmployeeId == duration.EmployeeId)
                .Where(x => !duration.ExcludeEntryIds.Contains(x.Id))
                .Where(x => x.StartTime < duration.EndTime && x.EndTime > duration.StartTime)
                .OrderBy(x => x.StartTime)
                .FirstOrDefaultAsync();

            if (overlappedEntry != null)
            {
                var errorMessage = OverlapErrorMessageFormatter.GetErrorMessage(
                    new TrackedEntryBase
                    {
                        Type = duration.EntryType,
                        StartTime = duration.StartTime,
                        EndTime = duration.EndTime
                    },
                    overlappedEntry
                );

                throw new ConflictingTimeRangeException(errorMessage);
            }
        }
    }
}



