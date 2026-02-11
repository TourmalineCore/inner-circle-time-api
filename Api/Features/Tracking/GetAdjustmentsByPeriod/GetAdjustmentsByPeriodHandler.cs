using Application.Queries;

namespace Api.Features.Tracking.GetAdjustmentsByPeriod;

public class GetAdjustmentsByPeriodHandler
{
    private readonly GetAdjustmentsByPeriodQuery _getAdjustmentsByPeriodQuery;

    public GetAdjustmentsByPeriodHandler(
        GetAdjustmentsByPeriodQuery getAdjustmentsByPeriodQuery
    )
    {
        _getAdjustmentsByPeriodQuery = getAdjustmentsByPeriodQuery;
    }

    public async Task<GetAdjustmentsByPeriodResponse> HandleAsync(
        DateOnly startDate,
        DateOnly endDate
    )
    {
        var adjustmentsByPeriod = await _getAdjustmentsByPeriodQuery.GetByPeriodAsync(
            startDate,
            endDate
        );

        return new GetAdjustmentsByPeriodResponse
        {
            Adjustments = adjustmentsByPeriod
                .Select(adjustment =>
                    new AdjustmentDto
                    {
                        Id = adjustment.Id,
                        Type = adjustment.Type,
                        StartTime = adjustment.StartTime,
                        EndTime = adjustment.EndTime,
                    }
                )
                .ToList()
        };
    }
}
