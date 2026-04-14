using Application.ExternalDeps.EmployeesApi;
using Core;
using Moq;
using Xunit;

namespace Application.Features.Reporting.GetAllEmployees;

[UnitTest]
public class GetAllEmployeesHandlerTests
{
    [Fact]
    public async Task GetAllEmployeesHandler_ShouldReturnEmployeesList()
    {
        var employee = new EmployeeDto
        {
            Id = 1,
            FullName = "Test Test Test",
        };

        var employeesApiMock = new Mock<IEmployeesApi>();

        employeesApiMock
            .Setup(x => x.GetAllEmployeesAsync())
            .ReturnsAsync(new EmployeesResponse
            {
                Employees = new List<EmployeeDto>
                {
                    employee
                }
            });

        var getAllEmployeesHandler = new GetAllEmployeesHandler(employeesApiMock.Object);

        var result = await getAllEmployeesHandler.HandleAsync();

        Assert.Equal(employee.Id, result.Employees[0].Id);
        Assert.Equal(employee.FullName, result.Employees[0].FullName);
    }
}
