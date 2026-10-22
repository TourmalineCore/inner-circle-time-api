using Application;
using Application.Exceptions;
using Application.Features.Tracking.Validators;
using Core.Features.Tracking.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

public class EntryDurationToCheck
{
    public required long EmployeeId { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public required EntryType EntryType { get; set; }

    public IReadOnlyList<long> ExcludeEntryIds { get; set; } = [];
}

public abstract class DbValidationEntryCommandBase<TRequest>
    where TRequest : class
{
    private const string CK_ENTRIES_END_TIME_IS_GREATER_THAN_START_TIME = "ck_entries_end_time_is_greater_than_start_time";
    private const string CK_ENTRIES_TIME_NO_SECONDS = "ck_entries_time_no_seconds";
    private IReadOnlyList<string> _overlapConstraints { get; } = new List<string>
    {
        // Task can overlap with Make-up time, Sick leave and Vacation, but it cannot overlap with all others
        $"ck_entries_{(int)EntryType.Task}_{(int)EntryType.Unwell}_{(int)EntryType.AwayWithMakeUpTime}_no_time_overlap",
        // Make-up time can overlap with Vacation and Task, but it cannot overlap with all others
        $"ck_entries_{(int)EntryType.Unwell}_{(int)EntryType.AwayWithMakeUpTime}_{(int)EntryType.MakeUpTime}_{(int)EntryType.SickLeave}_no_time_overlap",
        // Unwell, Away with make-up time, Sick leave, Vacation cannot overlap
        $"ck_entries_{(int)EntryType.Unwell}_{(int)EntryType.AwayWithMakeUpTime}_{(int)EntryType.SickLeave}_{(int)EntryType.Vacation}_no_time_overlap"
    };

    private readonly EntryOverlapValidator _entryOverlapValidator;

    protected DbValidationEntryCommandBase(
        TenantAppDbContext context
    )
    {
        _entryOverlapValidator = new EntryOverlapValidator(context);
    }

    public async Task<long> MakeChangesInDbAsync(TRequest request)
    {
        try
        {
            return await MakeChangesToEntryAsync(request);
        }
        // Double checking is necessary because different operations (SaveChangesAsync, ExecuteUpdateAsync) generate different exceptions
        // Depending on the type of exception, the ConstraintName can be both in the exception and in the internal exception 
        catch (Exception e) when (
            (e.InnerException as PostgresException)?.ConstraintName == CK_ENTRIES_END_TIME_IS_GREATER_THAN_START_TIME ||
            (e as PostgresException)?.ConstraintName == CK_ENTRIES_END_TIME_IS_GREATER_THAN_START_TIME
        )
        {
            throw new InvalidTimeRangeException(
                "End time must be greater than start time",
                e
            );
        }
        catch (Exception e) when (
            _overlapConstraints.Contains((e.InnerException as PostgresException)?.ConstraintName) ||
            _overlapConstraints.Contains((e as PostgresException)?.ConstraintName)
        )
        {
            var durationsToCheck = await GetEntryDurationsToCheckAsync(request);

            await _entryOverlapValidator.ValidateOverlapsAsync(durationsToCheck);

            throw new ConflictingTimeRangeException(
                "Another entry is already tracked for this time. Adjust the time or edit the existing entry.",
                e
            );
        }
        catch (Exception e) when (
            (e.InnerException as PostgresException)?.ConstraintName == CK_ENTRIES_TIME_NO_SECONDS ||
            (e as PostgresException)?.ConstraintName == CK_ENTRIES_TIME_NO_SECONDS
        )
        {
            throw new TimeNoSecondsException(
                "An unexpected error has occurred with the time format, try to add the record in the system again",
                e
            );
        }
    }

    protected abstract Task<long> MakeChangesToEntryAsync(TRequest request);

    protected abstract Task<IReadOnlyList<EntryDurationToCheck>> GetEntryDurationsToCheckAsync(TRequest request);
}
