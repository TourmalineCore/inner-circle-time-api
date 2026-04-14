namespace Application.ExternalDeps.EmployeesApi;

public interface IEmployeesApi
{
    Task<EmployeesResponse> GetAllEmployeesAsync();
}
