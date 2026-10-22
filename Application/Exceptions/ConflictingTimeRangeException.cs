namespace Application.Exceptions;

public class ConflictingTimeRangeException : Exception
{
    public ConflictingTimeRangeException(string message, Exception inner) : base(message, inner) { }

    public ConflictingTimeRangeException(string message) : base(message) { }
}
