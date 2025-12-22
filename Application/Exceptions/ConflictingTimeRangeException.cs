public class ConflictingTimeRangeException : Exception
{
    public ConflictingTimeRangeException() { }
    public ConflictingTimeRangeException(string message) : base(message) { }
    public ConflictingTimeRangeException(string message, Exception inner) : base(message, inner) { }
}
