using Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Application;

internal static class TenantAppDbContextExtensionsTestsRelated
{
    public static TenantAppDbContext CreateInMemoryTenantContextForTests(long tenantId)
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new TenantAppDbContext(options, tenantId);

        // creates a database and tables based on our model
        context.Database.EnsureCreated();

        // we need to turn off PostgreSQL related check constraints in this mode since SQLite doesn't support them
        context.Database.ExecuteSqlRaw(@"
            PRAGMA ignore_check_constraints = ON;
        ");

        return context;
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
