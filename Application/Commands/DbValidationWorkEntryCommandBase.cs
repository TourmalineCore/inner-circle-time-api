using Npgsql;


// This class is necessary in order to avoid duplicate validation at the database level in teams based on a modified work record
public abstract class DbValidationWorkEntryCommandBase<TCommandParams>
    where TCommandParams : class
{
    public async Task<long> MakeChangesInDbAsync(TCommandParams commandParams)
    {
        try
        {
            return await MakeChangesToWorkEntryAsync(commandParams);
        }
        catch (Exception e) when (
            (e.InnerException as PostgresException)?.ConstraintName == "ck_work_entries_end_time_is_greater_than_start_time" ||
            (e as PostgresException)?.ConstraintName == "ck_work_entries_end_time_is_greater_than_start_time"
        )
        {
            throw new InvalidTimeRangeException(
                "End time must be greater than start time",
                e
            );
        }
        catch (Exception e) when (
            (e.InnerException as PostgresException)?.ConstraintName == "ck_work_entries_no_time_overlap" ||
            (e as PostgresException)?.ConstraintName == "ck_work_entries_no_time_overlap"
        )
        {
            throw new ConflictingTimeRangeException(
                "Another task is scheduled for this time",
                e
            );
        }
    }

    protected abstract Task<long> MakeChangesToWorkEntryAsync(TCommandParams commandParams);
}
