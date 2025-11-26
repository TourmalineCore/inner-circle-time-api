namespace Core.Entities;

public class Entity : EntityBase
{
    // EntityFrameworkCore related empty default constructor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Entity()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public long EntityProp { get; set; }
}
