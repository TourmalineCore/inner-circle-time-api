using Microsoft.VisualBasic;

namespace Core;

public abstract class EntityBase
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long EmployeeId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string TimeZoneId { get; set; }

    public DateInterval Duration { get; set; }

    public int Type { get; set; }

    public string Description { get; set; }

    public bool IsDeleted { get; set; }
}
