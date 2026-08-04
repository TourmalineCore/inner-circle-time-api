using Core;
using Core.Entities;
using Xunit;

namespace Application.Features.ToDos.Handlers.GetToDos;

[UnitTest]
public class GetToDosQueryTests
{
    private const long TENANT_ID = 777;

    [Fact]
    public async Task GetAsync_ShouldReturnToDosOrderedThatNewerAreFirst()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);

        var firstToDo = await context.AddEntityAndSaveAsync(new ToDo
        {
            Name = "First",
            TenantId = TENANT_ID,
            CreatedAtUtc = new DateTime(2026, 09, 01, 14, 30, 05, 356, DateTimeKind.Utc),
        });

        var secondToDo = await context.AddEntityAndSaveAsync(new ToDo
        {
            Name = "Second",
            TenantId = TENANT_ID,
            CreatedAtUtc = new DateTime(2026, 09, 01, 15, 30, 05, 356, DateTimeKind.Utc),
        });

        var thirdToDo = await context.AddEntityAndSaveAsync(new ToDo
        {
            Name = "Third",
            TenantId = TENANT_ID,
            CreatedAtUtc = new DateTime(2026, 09, 01, 16, 30, 05, 356, DateTimeKind.Utc),
        });

        var getToDosQuery = new GetToDosQuery(context);

        var toDos = await getToDosQuery.GetAsync();

        Assert.Equal(thirdToDo.Name, toDos[0].Name);
        Assert.Equal(secondToDo.Name, toDos[1].Name);
        Assert.Equal(firstToDo.Name, toDos[2].Name);
    }
}
