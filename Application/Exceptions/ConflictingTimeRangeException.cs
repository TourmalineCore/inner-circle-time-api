public class ConflictingTimeRangeException : Exception
{
    public ConflictingTimeRangeException(string message, Exception inner) : base(message, inner) { }
}
