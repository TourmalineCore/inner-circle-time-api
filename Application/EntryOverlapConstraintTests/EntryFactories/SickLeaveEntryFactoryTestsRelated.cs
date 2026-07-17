using Application;
using Application.EntryOverlapConstraintTests.EntryFactories;
using Application.Features.Tracking.CreateSickLeaveEntry;
using Application.Features.Tracking.UpdateSickLeaveEntry;
using Core.Entities;

public class SickLeaveFactoryTestsRelated : EntryOverlapFactoryTestsRelated
{
    public override TrackedEntryBase CreateEntry(DateTime startTime, DateTime endTime)
    {
        return new SickLeaveEntry
        {
            EmployeeId = employeeId,
            StartTime = startTime,
            EndTime = endTime,
        };
    }

    public override Func<TenantAppDbContext, IClaimsProvider, Task> CreateEntryCommand()
    {
        return (context, claimsProvider) =>
            new CreateSickLeaveEntryCommand(context, claimsProvider)
                .ExecuteAsync(new CreateSickLeaveEntryRequest
                {
                    Period = new PeriodDto
                    {
                        StartDate = DateOnly.FromDateTime(createTestStartTime),
                        EndDate = DateOnly.FromDateTime(createTestEndTime),
                    }
                });
    }

    public override Func<TenantAppDbContext, IClaimsProvider, long, Task> UpdateEntryCommand()
    {
        return (context, claimsProvider, entryId) =>
            new UpdateSickLeaveEntryCommand(context, claimsProvider)
                .ExecuteAsync(new UpdateSickLeaveEntryRequest
                {
                    Id = entryId,
                    Period = new PeriodDto
                    {
                        StartDate = DateOnly.FromDateTime(updateTestStartTime),
                        EndDate = DateOnly.FromDateTime(updateTestEndTime),
                    }
                });
    }
}
