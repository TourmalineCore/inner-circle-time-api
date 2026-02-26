using Npgsql;

public abstract class DbValidationEntryCommandBase<TRequest>
    where TRequest : class
{
    private const string CK_ENTRIES_END_TIME_IS_GREATER_THAN_START_TIME = "ck_entries_end_time_is_greater_than_start_time";
    private const string CK_WORK_ENTRIES_NO_TIME_OVERLAP = "ck_work_entries_no_time_overlap";

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
            (e.InnerException as PostgresException)?.ConstraintName == CK_WORK_ENTRIES_NO_TIME_OVERLAP ||
            (e as PostgresException)?.ConstraintName == CK_WORK_ENTRIES_NO_TIME_OVERLAP
        )
        {
            throw new ConflictingTimeRangeException(
                "Another task is scheduled for this time",
                e
            );
        }
    }

    protected abstract Task<long> MakeChangesToEntryAsync(TRequest request);
}
