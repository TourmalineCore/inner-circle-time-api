using Application;
using Application.EntryOverlapConstraintTests.EntryFactories;
using Application.Features.Tracking.CreateVacationEntry;
using Application.Features.Tracking.UpdateVacationEntry;
using Core.Entities;

public class VacationEntryFactoryTestsRelated : EntryOverlapFactoryTestsRelated
{
    public override TrackedEntryBase CreateEntry(DateTime startTime, DateTime endTime)
    {
        return new VacationEntry
        {
            EmployeeId = employeeId,
            StartTime = startTime,
            EndTime = endTime,
            IsUnpaid = false
        };
    }

    public override Func<TenantAppDbContext, IClaimsProvider, Task> CreateEntryCommand()
    {
        return (context, claimsProvider) =>
            new CreateVacationEntryCommand(context, claimsProvider)
                .ExecuteAsync(new CreateVacationEntryRequest
                {
                    Period = new PeriodDto
                    {
                        StartDate = DateOnly.FromDateTime(createTestStartTime),
                        EndDate = DateOnly.FromDateTime(createTestEndTime),
                    },
                    IsUnpaid = false
                });
    }

    public override Func<TenantAppDbContext, IClaimsProvider, long, Task> UpdateEntryCommand()
    {
        return (context, claimsProvider, entryId) =>
            new UpdateVacationEntryCommand(context, claimsProvider)
                .ExecuteAsync(new UpdateVacationEntryRequest
                {
                    Id = entryId,
                    Period = new PeriodDto
                    {
                        StartDate = DateOnly.FromDateTime(updateTestStartTime),
                        EndDate = DateOnly.FromDateTime(updateTestEndTime),
                    },
                    IsUnpaid = false
                });
    }
}
