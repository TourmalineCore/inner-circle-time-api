using Application;
using Application.Features.Tracking.CreateUnwellEntry;
using Application.Features.Tracking.EntryOverlapConstraintTests.EntryFactories;
using Application.Features.Tracking.UpdateUnwellEntry;
using Core.Entities;

public class UnwellEntryTestFactory : EntryOverlapTestFactory
{
    public override TrackedEntryBase CreateEntry(DateTime startTime, DateTime endTime)
    {
        return new UnwellEntry
        {
            EmployeeId = employeeId,
            StartTime = startTime,
            EndTime = endTime,
        };
    }

    public override Func<TenantAppDbContext, IClaimsProvider, Task> CreateEntryCommand()
    {
        return (context, claimsProvider) =>
            new CreateUnwellEntryCommand(context, claimsProvider)
                .ExecuteAsync(new CreateUnwellEntryRequest
                {
                    StartTime = createTestStartTime,
                    EndTime = createTestEndTime,
                });
    }

    public override Func<TenantAppDbContext, IClaimsProvider, long, Task> UpdateEntryCommand()
    {
        return (context, claimsProvider, entryId) =>
            new UpdateUnwellEntryCommand(context, claimsProvider)
                .ExecuteAsync(new UpdateUnwellEntryRequest
                {
                    Id = entryId,
                    StartTime = updateTestStartTime,
                    EndTime = updateTestEndTime,
                });
    }
}
