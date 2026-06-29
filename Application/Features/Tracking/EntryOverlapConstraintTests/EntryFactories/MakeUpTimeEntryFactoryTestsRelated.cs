using Application;
using Application.Features.Tracking.CreateAwayWithMakeUpTimeEntry;
using Application.Features.Tracking.EntryOverlapConstraintTests.EntryFactories;
using Application.Features.Tracking.UpdateAwayWithMakeUpTimeEntry;
using Core.Entities;

public class MakeUpTimeEntryFactoryTest : EntryOverlapFactoryTest
{
    public override TrackedEntryBase CreateEntry(DateTime startTime, DateTime endTime)
    {
        // MakeUpTimeEntry requires a linked record to be saved.
        // Create it together with the "Away With Make Up Time Entry".
        return new AwayWithMakeUpTimeEntry
        {
            EmployeeId = employeeId,
            // Add one day to avoid any overlaps.
            StartTime = startTime.AddDays(+1),
            EndTime = endTime.AddDays(+1),
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
                    // Add one day to avoid any overlaps.
                    StartTime = createTestStartTime.AddDays(+1),
                    EndTime = createTestEndTime.AddDays(+1),
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
                    // Add one day to avoid any overlaps.
                    StartTime = updateTestStartTime.AddDays(+1),
                    EndTime = updateTestEndTime.AddDays(+1),
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
