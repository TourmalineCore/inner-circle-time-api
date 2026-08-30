using Core;
using Microsoft.EntityFrameworkCore;

namespace Application;

internal static class AppDbContextExtensionsTestsRelated
{
    public static AppDbContext CreateInMemoryContextForTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(
                databaseName: new Random().Next().ToString(),
                x => x.EnableNullChecks(false)
            )
            .Options;

        return new AppDbContext(
            options
        );
    }

    public async static Task<ToDo> AddEntityAndSaveAsync<ToDo>(
        this AppDbContext context,
        ToDo newEntity
    )
    {
        await context
            .Set<ToDo>()
            .AddAsync(newEntity);

        await context.SaveChangesAsync();

        return newEntity;
    }
}
