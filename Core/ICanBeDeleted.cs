namespace Core;

public interface ICanBeDeleted
{
    public DateTime? DeletedAtUtc { get; set; }
}
