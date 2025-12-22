public class InvalidTimeRangeException : Exception
{
    public InvalidTimeRangeException() { }
    public InvalidTimeRangeException(string message) : base(message) { }
    public InvalidTimeRangeException(string message, Exception inner) : base(message, inner) { }
}
