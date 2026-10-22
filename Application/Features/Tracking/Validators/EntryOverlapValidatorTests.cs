using Application.Exceptions;
using Core;
using Core.Features.Tracking.Entities;
using Xunit;

namespace Application.Features.Tracking.Validators;

[IntegrationTest]
public class EntryOverlapValidatorTests : IntegrationTestBase
{
    [Fact]
    public async Task ValidateOverlapsAsync_WhenNoOverlap_ShouldNotThrowConflictingTimeRangeException()
    {
        var context = CreateTenantDbContext();

        await AddEntityAndSaveAsync(context, new TaskEntry
        {
            TenantId = TENANT_ID,
            EmployeeId = 1,
            StartTime = new DateTime(2026, 10, 21, 9, 0, 0),
            EndTime = new DateTime(2026, 10, 21, 10, 0, 0)
        });

        var validator = new EntryOverlapValidator(context);

        var durations = new List<EntryDurationToCheck>
        {
            new EntryDurationToCheck
            {
                EmployeeId = 1,
                StartTime = new DateTime(2026, 10, 21, 11, 0, 0),
                EndTime = new DateTime(2026, 10, 21, 12, 0, 0),
                EntryType = EntryType.Task
            }
        };

        var exception = await Record.ExceptionAsync(
            () => validator.ValidateOverlapsAsync(durations)
        );

        Assert.Null(exception);
    }

    [Fact]
    public async Task ValidateOverlapsAsync_WhenOverlapExists_ShouldThrowConflictingTimeRangeException()
    {
        var context = CreateTenantDbContext();

        await AddEntityAndSaveAsync(context, new TaskEntry
        {
            TenantId = TENANT_ID,
            EmployeeId = 1,
            StartTime = new DateTime(2026, 10, 21, 9, 0, 0),
            EndTime = new DateTime(2026, 10, 21, 10, 0, 0)
        });

        var validator = new EntryOverlapValidator(context);

        var durations = new List<EntryDurationToCheck>
        {
            new EntryDurationToCheck
            {
                EmployeeId = 1,
                StartTime = new DateTime(2026, 10, 21, 9, 30, 0),
                EndTime = new DateTime(2026, 10, 21, 10, 30, 0),
                EntryType = EntryType.Task
            }
        };

        await Assert.ThrowsAsync<ConflictingTimeRangeException>(
            () => validator.ValidateOverlapsAsync(durations));
    }

    [Fact]
    public async Task ValidateOverlapsAsync_WhenEntryIdMatchesOverlappingEntry_ShouldNotThrowConflictingTimeRangeException()
    {
        var context = CreateTenantDbContext();

        var taskEntry = await AddEntityAndSaveAsync(context, new TaskEntry
        {
            TenantId = TENANT_ID,
            EmployeeId = 1,
            StartTime = new DateTime(2026, 10, 21, 9, 0, 0),
            EndTime = new DateTime(2026, 10, 21, 10, 0, 0)
        });

        var validator = new EntryOverlapValidator(context);

        var durations = new List<EntryDurationToCheck>
        {
            new EntryDurationToCheck
            {
                EmployeeId = 1,
                StartTime = new DateTime(2026, 10, 21, 9, 0, 0),
                EndTime = new DateTime(2026, 10, 21, 10, 0, 0),
                EntryType = EntryType.Task,
                ExcludeEntryIds = [taskEntry.Id]
            }
        };

        var exception = await Record.ExceptionAsync(
            () => validator.ValidateOverlapsAsync(durations)
        );

        Assert.Null(exception);
    }

    [Fact]
    public async Task ValidateOverlapsAsync_WhenMultipleOverlaps_ShouldPickFirstByStartTime()
    {
        var context = CreateTenantDbContext();

        await AddEntityAndSaveAsync(context, new TaskEntry
        {
            TenantId = TENANT_ID,
            EmployeeId = 1,
            StartTime = new DateTime(2026, 10, 21, 9, 0, 0),
            EndTime = new DateTime(2026, 10, 21, 10, 0, 0)
        });

        await AddEntityAndSaveAsync(context, new TaskEntry
        {
            TenantId = TENANT_ID,
            EmployeeId = 1,
            StartTime = new DateTime(2026, 10, 21, 10, 0, 0),
            EndTime = new DateTime(2026, 10, 21, 10, 30, 0)
        });

        await context.SaveChangesAsync();

        var validator = new EntryOverlapValidator(context);

        var durations = new List<EntryDurationToCheck>
        {
            new EntryDurationToCheck
            {
                EmployeeId = 1,
                StartTime = new DateTime(2026, 10, 21, 9, 30, 0),
                EndTime = new DateTime(2026, 10, 21, 11, 00, 0),
                EntryType = EntryType.Task
            }
        };

        var exception = await Assert.ThrowsAsync<ConflictingTimeRangeException>(
            () => validator.ValidateOverlapsAsync(durations));

        Assert.Contains("09:00-10:00", exception.Message);
    }

    [Fact]
    public async Task ValidateOverlapsAsync_WhenOverlapExistsForDifferentEmployee_ShouldNotThrowConflictingTimeRangeException()
    {
        var context = CreateTenantDbContext();

        const long OTHER_EMPLOYEE_ID = -1;

        await AddEntityAndSaveAsync(context, new TaskEntry
        {
            TenantId = TENANT_ID,
            EmployeeId = 1,
            StartTime = new DateTime(2026, 10, 21, 9, 0, 0),
            EndTime = new DateTime(2026, 10, 21, 10, 0, 0)
        });

        var validator = new EntryOverlapValidator(context);

        var durations = new List<EntryDurationToCheck>
        {
            new EntryDurationToCheck
            {
                EmployeeId = OTHER_EMPLOYEE_ID,
                StartTime = new DateTime(2026, 10, 21, 9, 0, 0),
                EndTime = new DateTime(2026, 10, 21, 10, 0, 0),
                EntryType = EntryType.Task
            }
        };

        var exception = await Record.ExceptionAsync(
            () => validator.ValidateOverlapsAsync(durations));

        Assert.Null(exception);
    }

    [Fact]
    public async Task ValidateOverlapsAsync_WhenOverlapExistsForDifferentTenant_ShouldNotThrowConflictingTimeRangeException()
    {
        var context = CreateTenantDbContext();

        const long OTHER_TENANT_ID = -1;

        await AddEntityAndSaveAsync(context, new TaskEntry
        {
            TenantId = OTHER_TENANT_ID,
            EmployeeId = 1,
            StartTime = new DateTime(2026, 10, 21, 9, 0, 0),
            EndTime = new DateTime(2026, 10, 21, 10, 0, 0)
        });

        var validator = new EntryOverlapValidator(context);

        var durations = new List<EntryDurationToCheck>
        {
            new EntryDurationToCheck
            {
                EmployeeId = 1,
                StartTime = new DateTime(2026, 10, 21, 9, 0, 0),
                EndTime = new DateTime(2026, 10, 21, 10, 0, 0),
                EntryType = EntryType.Task
            }
        };

        var exception = await Record.ExceptionAsync(
            () => validator.ValidateOverlapsAsync(durations));

        Assert.Null(exception);
    }
}
