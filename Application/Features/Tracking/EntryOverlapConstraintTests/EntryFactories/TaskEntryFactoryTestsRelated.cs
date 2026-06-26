using Application;
using Application.Features.Tracking.CreateTaskEntry;
using Application.Features.Tracking.EntryOverlapConstraintTests.EntryFactories;
using Application.Features.Tracking.UpdateTaskEntry;
using Core.Entities;

public class TaskEntryFactoryTest : EntryOverlapFactoryTest
{
    public override TrackedEntryBase CreateEntry(DateTime startTime, DateTime endTime)
    {
        return new TaskEntry
        {
            EmployeeId = employeeId,
            Title = "Existing Task",
            StartTime = startTime,
            EndTime = endTime,
            TaskId = "#2231",
            ProjectId = 1,
            Description = "Task description",
        };
    }

    public override Func<TenantAppDbContext, IClaimsProvider, Task> CreateEntryCommand()
    {
        return (context, claimsProvider) =>
            new CreateTaskEntryCommand(context, claimsProvider)
                .ExecuteAsync(new CreateTaskEntryRequest
                {
                    Title = "New Task",
                    StartTime = createTestStartTime,
                    EndTime = createTestEndTime,
                    TaskId = "#222",
                    ProjectId = 1,
                    Description = "Description"
                });
    }

    public override Func<TenantAppDbContext, IClaimsProvider, long, Task> UpdateEntryCommand()
    {
        return (context, claimsProvider, entryId) =>
            new UpdateTaskEntryCommand(context, claimsProvider)
                .ExecuteAsync(new UpdateTaskEntryRequest
                {
                    Id = entryId,
                    Title = "New Task",
                    StartTime = updateTestStartTime,
                    EndTime = updateTestEndTime,
                    TaskId = "#222",
                    ProjectId = 1,
                    Description = "Description"
                });
    }
}
