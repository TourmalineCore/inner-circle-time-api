using Application;
using Application.EntryOverlapConstraintTests.EntryFactories;
using Application.Features.Tracking.Handlers.CreateSickLeaveEntry;
using Application.Features.Tracking.Handlers.UpdateSickLeaveEntry;
using Core.Features.Tracking.Entities;

public class SickLeaveEntryFactoryTestsRelated : EntryOverlapFactoryTestsRelated
{
    public override TrackedEntryBase CreateEntry(DateTime startTime, DateTime endTime)
    {
        return new SickLeaveEntry
        {
            TenantId = tenantId,
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
