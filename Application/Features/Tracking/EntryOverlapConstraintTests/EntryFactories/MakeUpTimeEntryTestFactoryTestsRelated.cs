using Application;
using Application.Features.Tracking.CreateAwayWithMakeUpTimeEntry;
using Application.Features.Tracking.EntryOverlapConstraintTests.EntryFactories;
using Application.Features.Tracking.UpdateAwayWithMakeUpTimeEntry;
using Core.Entities;

public class MakeUpTimeEntryTestFactory : EntryOverlapTestFactory
{
    public override TrackedEntryBase CreateEntry(DateTime startTime, DateTime endTime)
    {
        // MakeUpTimeEntry requires a linked record to be saved.
        // Create it together with the "Away With Make Up Time Entry".
        return new AwayWithMakeUpTimeEntry
        {
            EmployeeId = employeeId,
            // Subtract one week to prevent any overlap.
            StartTime = startTime.AddDays(-7),
            EndTime = endTime.AddDays(-7),
            Description = "Description",
            MakeUpTimeList = [
                new MakeUpTimeEntry
                {
                    TenantId = tenantId,
                    EmployeeId = employeeId,
                    StartTime = startTime,
                    EndTime = endTime
                }
            ]
        };
    }

    public override Func<TenantAppDbContext, IClaimsProvider, Task> CreateEntryCommand()
    {
        return (context, claimsProvider) =>
            new CreateAwayWithMakeUpTimeEntryCommand(context, claimsProvider)
                .ExecuteAsync(new CreateAwayWithMakeUpTimeEntryRequest
                {
                    // Subtract one week to prevent any overlap.
                    StartTime = createTestStartTime.AddDays(-7),
                    EndTime = createTestEndTime.AddDays(-7),
                    Description = "Description",
                    MakeUpTimeList = [
                        new MakeUpTimeEntryDto
                        {
                            StartTime = createTestStartTime,
                            EndTime = createTestEndTime,
                        }
                    ]
                });
    }

    public override Func<TenantAppDbContext, IClaimsProvider, long, Task> UpdateEntryCommand()
    {
        return (context, claimsProvider, entryId) =>
           new UpdateAwayWithMakeUpTimeEntryCommand(context, claimsProvider)
                .ExecuteAsync(new UpdateAwayWithMakeUpTimeEntryRequest
                {
                    Id = entryId,
                    // Subtract one week to prevent any overlap.
                    StartTime = updateTestStartTime.AddDays(-7),
                    EndTime = updateTestEndTime.AddDays(-7),
                    Description = "Description",
                    MakeUpTimeList = [
                        new MakeUpTimeEntryDto
                        {
                            StartTime = updateTestStartTime,
                            EndTime = updateTestEndTime,
                        }
                    ]
                });
    }
}
