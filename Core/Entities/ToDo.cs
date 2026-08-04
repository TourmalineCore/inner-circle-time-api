
namespace Core.Entities;

public class ToDo : EntityBase, ICanBeDeleted
{
    public ToDo()
    {
    }

    public required string Name { get; set; }

    public required DateTime CreatedAtUtc { get; set; }

    public DateTime? DeletedAtUtc { get; set; }

    public ToDoStatus GetStatus(IDateTimeProvider dateTimeProvider)
    {
        var utcNow = dateTimeProvider.UtcNow;

        return this switch
        {
            ToDo t when t.CreatedAtUtc > utcNow.AddDays(-7) && t.CreatedAtUtc < utcNow => ToDoStatus.New,
            ToDo t when t.CreatedAtUtc > utcNow.AddDays(-28) && t.CreatedAtUtc <= utcNow.AddDays(-7) => ToDoStatus.Old,
            ToDo t when t.CreatedAtUtc <= utcNow.AddDays(-28) => ToDoStatus.Forgotten,
            _ => throw new ArgumentOutOfRangeException($"Not expected path of ${nameof(GetStatus)}"),
        };
    }
}
