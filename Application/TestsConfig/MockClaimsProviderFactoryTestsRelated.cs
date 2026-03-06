using Application;
using Moq;

public static class MockClaimsProviderFactory
{
    public static IClaimsProvider CreateMock(long employeeId, long tenantId)
    {
        var mockClaimsProvider = new Mock<IClaimsProvider>();

        mockClaimsProvider
            .Setup(x => x.EmployeeId)
            .Returns(employeeId);

        mockClaimsProvider
            .Setup(x => x.TenantId)
            .Returns(tenantId);

        return mockClaimsProvider.Object;
    }
}
