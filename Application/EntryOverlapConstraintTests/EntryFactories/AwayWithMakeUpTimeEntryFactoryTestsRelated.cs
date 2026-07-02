using Application;
using Application.EntryOverlapConstraintTests.EntryFactories;
using Application.Features.Tracking.CreateAwayWithMakeUpTimeEntry;
using Application.Features.Tracking.UpdateAwayWithMakeUpTimeEntry;
using Core.Entities;

public class AwayWithMakeUpTimeEntryFactoryTestsRelated : EntryOverlapFactoryTestsRelated
{
    public override TrackedEntryBase CreateEntry(DateTime startTime, DateTime endTime)
    {
        return new AwayWithMakeUpTimeEntry
        {
            EmployeeId = employeeId,
            StartTime = startTime,
            EndTime = endTime,
            Description = "Description",
            MakeUpTimeList = []
        };
    }

    public override Func<TenantAppDbContext, IClaimsProvider, Task> CreateEntryCommand()
    {
        return (context, claimsProvider) =>
            new CreateAwayWithMakeUpTimeEntryCommand(context, claimsProvider)
                .ExecuteAsync(new CreateAwayWithMakeUpTimeEntryRequest
                {
                    StartTime = createTestStartTime,
                    EndTime = createTestEndTime,
                    Description = "Description",
                    MakeUpTimeList = []
                });
    }

    public override Func<TenantAppDbContext, IClaimsProvider, long, Task> UpdateEntryCommand()
    {
        return (context, claimsProvider, entryId) =>
           new UpdateAwayWithMakeUpTimeEntryCommand(context, claimsProvider)
                .ExecuteAsync(new UpdateAwayWithMakeUpTimeEntryRequest
                {
                    Id = entryId,
                    StartTime = updateTestStartTime,
                    EndTime = updateTestEndTime,
                    Description = "Description",
                    MakeUpTimeList = []
                });
    }
}
