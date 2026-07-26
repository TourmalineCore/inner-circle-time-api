
namespace Core.Entities;

public class ToDo : EntityBase, ICanBeDeleted
{
    public ToDo()
    {
    }

    public required string Name { get; set; }

    public DateTime? DeletedAtUtc { get; set; }
}
