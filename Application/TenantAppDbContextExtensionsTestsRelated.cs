using Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Application;

internal static class TenantAppDbContextExtensionsTestsRelated
{
    public async static Task<(TenantAppDbContext Context, SqliteConnection Connection)> CreateSqlInMemoryTenantContextForTestsAsync(long tenantId)
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new TenantAppDbContext(options, tenantId);

        // creates a database and tables based on our model
        await context.Database.EnsureCreatedAsync();

        // off check constraints
        await context.Database.ExecuteSqlRawAsync(@"
            PRAGMA ignore_check_constraints = ON;
        ");

        return (context, connection);
    }

    public async static Task<TEntity> AddEntityAndSaveAsync<TEntity>(
        this TenantAppDbContext context,
        TEntity newEntity
    )
        where TEntity : EntityBase
    {
        await context
            .Set<TEntity>()
            .AddAsync(newEntity);

        await context.SaveChangesAsync();

        return newEntity;
    }
}
