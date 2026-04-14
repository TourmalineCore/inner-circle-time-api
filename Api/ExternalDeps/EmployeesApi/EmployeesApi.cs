using Application.ExternalDeps.EmployeesApi;
using Microsoft.Extensions.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

namespace Api.ExternalDeps.EmployeesApi;

public class EmployeesApi : IEmployeesApi
{
    private readonly ExternalDepsUrls _externalDepsUrls;
    private readonly AuthenticationOptions _authenticationOptions;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EmployeesApi(
        IOptions<ExternalDepsUrls> externalDepsUrls,
        IOptions<AuthenticationOptions> authenticationOptions,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _externalDepsUrls = externalDepsUrls.Value;
        _authenticationOptions = authenticationOptions.Value;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<EmployeesResponse> GetAllEmployeesAsync()
    {
        var link = $"{_externalDepsUrls.EmployeesApiRootUrl}/internal/get-employees";

        var headerName = _authenticationOptions.IsDebugTokenEnabled
          ? "X-DEBUG-TOKEN"
          : "Authorization";

        var token = _httpContextAccessor
          .HttpContext!
          .Request
          .Headers[headerName]
          .ToString();

        // ToDo improve work with HttpClient
        // https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines
        using var httpClient = new HttpClient()!;

        httpClient.DefaultRequestHeaders.Add(headerName, token);

        var employeesDtos = await httpClient.GetFromJsonAsync<List<EmployeeDto>>(link);

        return new EmployeesResponse
        {
            Employees = employeesDtos!
        };
    }
}
