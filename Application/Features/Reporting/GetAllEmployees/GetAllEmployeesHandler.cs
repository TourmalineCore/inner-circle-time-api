using Application.ExternalDeps.EmployeesApi;

namespace Application.Features.Reporting.GetAllEmployees;

public class GetAllEmployeesHandler
{
    private readonly IEmployeesApi _employeesApi;

    public GetAllEmployeesHandler(
        IEmployeesApi employeesApi
    )
    {
        _employeesApi = employeesApi;
    }

    public async Task<GetAllEmployeesResponse> HandleAsync()
    {
        var allEmployeesResponse = await _employeesApi.GetAllEmployeesAsync();

        return new GetAllEmployeesResponse
        {
            Employees = allEmployeesResponse.Employees
        };
    }
}
