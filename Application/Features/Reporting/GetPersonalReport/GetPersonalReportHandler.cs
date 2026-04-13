using Core.Entities;

namespace Application.Features.Reporting.GetPersonalReport;

public class GetPersonalReportHandler
{
    public GetPersonalReportHandler()
    {
    }

    public async Task<GetPersonalReportResponse> HandleAsync()
    {
        return new GetPersonalReportResponse
        {
            TrackedEntries = new List<TrackedEntryDto> { },
            TaskHours = 0,
            UnwellHours = 0
        };
    }
}
