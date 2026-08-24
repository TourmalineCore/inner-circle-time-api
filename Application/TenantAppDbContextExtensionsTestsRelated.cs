using Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Application;

internal static class TenantAppDbContextExtensionsTestsRelated
{
    public static TenantAppDbContext CreateInMemoryTenantContextForTests(long tenantId)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(
                // we need a unique db name so that tests of the same collection can run in isolation
                // otherwise they inrefere and see each others data
                new Random().Next().ToString(),
                // we want to provide as little setup data as possible to check a certain piece of a flow
                // thus, we don't want to specify all properties of seeded data when it isn't used by the logic
                // for instance, I need to check that an entity exists by Id, I don't need to setup its required Name property
                // this option allows me to bypass requited non-nullable Name check
                x => x.EnableNullChecks(false)
            )
            .Options;

        return new TenantAppDbContext(
            options,
            tenantId
        );
    }

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
