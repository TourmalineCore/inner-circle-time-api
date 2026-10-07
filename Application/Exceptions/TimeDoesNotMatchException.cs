namespace Application.Exceptions;

public class TimeDoesNotMatchException : Exception
{
    public TimeDoesNotMatchException(string message) : base(message) { }
}
