namespace Application.Exceptions;

public class TimeNoSecondsException : Exception
{
    public TimeNoSecondsException(string message, Exception inner) : base(message, inner) { }
}
