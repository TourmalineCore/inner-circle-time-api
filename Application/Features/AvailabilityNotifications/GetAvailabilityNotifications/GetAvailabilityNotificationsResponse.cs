using Core.Features.AvailabilityNotifications.Entities;
using Core.Features.Tracking.Entities;

namespace Application.Features.AvailabilityNotifications.GetAvailabilityNotifications;

public class GetAvailabilityNotificationsResponse
{
    public required long Id { get; set; }

    public required long EntryId { get; set; }

    public required string EmployeeFullName { get; set; }

    public required EntryType EntryType { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public string? Description { get; set; }

    public List<AdjustmentTime>? AdjustmentTimeList { get; set; }

    public AvailabilityNotificationStatus Status { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}

public class AdjustmentTime
{
    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }
}
