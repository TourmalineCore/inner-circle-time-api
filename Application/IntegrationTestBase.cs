using Application;
using Microsoft.EntityFrameworkCore;
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
        var connectionString = "Host=localhost;Port=7507;Database=inner-circle-time-api-db;Username=postgres;Password=postgres";

        _dbConnection = new NpgsqlConnection(connectionString);

        await _dbConnection.OpenAsync();

        await ApplyMigrationsAsync();

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

    private async Task ApplyMigrationsAsync()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(_dbConnection);

        using var context = new AppDbContext(optionsBuilder.Options);

        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            await context.Database.MigrateAsync();
        }
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
