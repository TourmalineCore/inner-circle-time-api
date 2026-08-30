
namespace Core.Entities;

public class ToDo : EntityBase
{
    public ToDo()
    {
    }

    public required string Name { get; set; }

    public required DateTime CreatedAtUtc { get; set; }

    public ToDoStatus GetStatus(IDateTimeProvider dateTimeProvider)
    {
        var utcNow = dateTimeProvider.UtcNow;

        return this switch
        {
            { CreatedAtUtc: var createdAtUtc } when createdAtUtc > utcNow.AddDays(-7) && createdAtUtc < utcNow => ToDoStatus.New,
            { CreatedAtUtc: var createdAtUtc } when createdAtUtc > utcNow.AddDays(-28) && createdAtUtc <= utcNow.AddDays(-7) => ToDoStatus.Old,
            { CreatedAtUtc: var createdAtUtc } when createdAtUtc <= utcNow.AddDays(-28) => ToDoStatus.Forgotten,
            _ => throw new ArgumentOutOfRangeException($"Not expected path of ${nameof(GetStatus)}"),
        };
    }
}
