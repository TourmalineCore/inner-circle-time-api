using Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Npgsql;
using Xunit;

public class IntegrationTestBase : IAsyncLifetime
{
    protected const long EMPLOYEE_ID = 1;
    protected const long TENANT_ID = 777;

    private NpgsqlConnection _dbConnection = null!;

    private DbContextOptions<AppDbContext> _dbContextOptions = null!;

    private NpgsqlTransaction _dbTransaction = null!;

    public async Task InitializeAsync()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../../../Api"))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        _dbConnection = new NpgsqlConnection(connectionString);

        await _dbConnection.OpenAsync();

        // Begin Transaction
        _dbTransaction = await _dbConnection.BeginTransactionAsync();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseNpgsql(_dbConnection);

        _dbContextOptions = optionsBuilder.Options;
    }


    // Rollback Transaction And Close Db Connection
    public async Task DisposeAsync()
    {
        if (_dbTransaction != null)
        {
            await _dbTransaction.RollbackAsync();
            await _dbTransaction.DisposeAsync();
        }

        if (_dbConnection != null)
            await _dbConnection.CloseAsync();
    }

    protected TenantAppDbContext CreateTenantDbContext()
    {
        return new TenantAppDbContext(
            _dbContextOptions,
            GetMockClaimsProvider()
        );
    }

    protected IClaimsProvider GetMockClaimsProvider()
    {
        var mockClaimsProvider = new Mock<IClaimsProvider>();

        mockClaimsProvider
            .Setup(cp => cp.EmployeeId)
            .Returns(EMPLOYEE_ID);

        mockClaimsProvider
            .Setup(cp => cp.TenantId)
            .Returns(TENANT_ID);

        return mockClaimsProvider.Object;
    }
}
