using Application;
using Application.EntryOverlapConstraintTests.EntryFactories;
using Application.Features.Tracking.CreateAwayWithMakeUpTimeEntry;
using Application.Features.Tracking.UpdateAwayWithMakeUpTimeEntry;
using Core.Entities;

public class MakeUpTimeEntryFactoryTestsRelated : EntryOverlapFactoryTestsRelated
{
    public override TrackedEntryBase CreateEntry(DateTime startTime, DateTime endTime)
    {
        // MakeUpTimeEntry requires a linked record to be saved.
        // Create it together with the "Away With Make Up Time Entry".
        return new AwayWithMakeUpTimeEntry
        {
            EmployeeId = employeeId,
            // Add two days to avoid any overlaps.
            StartTime = startTime.AddDays(+2),
            EndTime = endTime.AddDays(+2),
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
                    // Add two days to avoid any overlaps.
                    StartTime = createTestStartTime.AddDays(+2),
                    EndTime = createTestEndTime.AddDays(+2),
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
                    // Add two days to avoid any overlaps.
                    StartTime = updateTestStartTime.AddDays(+2),
                    EndTime = updateTestEndTime.AddDays(+2),
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
