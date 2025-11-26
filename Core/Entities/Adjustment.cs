using Microsoft.VisualBasic;

namespace Core.Entities;

public class Adjustment : EntityBase
{
    // EntityFrameworkCore related empty default constructor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Adjustment()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public long ParentId { get; set; }

    public DateInterval Amount { get; set; }

    public bool SickLeaveReason { get; set; }
    
    public bool IsApproved { get; set; }

    public bool IsPaid { get; set; }

    public bool IsFullDay { get; set; }
}
