using Core.Entities;

namespace Api.Features.Tracking.GetAdjustmentsByPeriod;

public class GetAdjustmentsByPeriodResponse
{
    public required List<AdjustmentDto> Adjustments { get; set; }
}

public class AdjustmentDto
{
    public required long Id { get; set; }

    public required EventType Type { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

}
