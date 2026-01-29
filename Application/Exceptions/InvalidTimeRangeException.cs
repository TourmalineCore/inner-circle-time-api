public class InvalidTimeRangeException : Exception
{
    public InvalidTimeRangeException(string message, Exception inner) : base(message, inner) { }
}
