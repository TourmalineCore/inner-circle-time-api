namespace Core;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
}
