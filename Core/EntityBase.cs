namespace Core;

public abstract class EntityBase
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long EmployeeId { get; set; }
}
