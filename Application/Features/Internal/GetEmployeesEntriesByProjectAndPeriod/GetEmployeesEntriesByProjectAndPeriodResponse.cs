using Core.Entities;

namespace Application.Features.Internal.GetEmployeesEntriesByProjectAndPeriod;

public class GetEmployeesEntriesByProjectAndPeriodResponse
{
    public required List<EmployeesEntriesDto> EmployeesEntries { get; set; }

}

public class EmployeesEntriesDto
{

    public required long EmployeeId { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

}
