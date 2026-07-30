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
        });

        var secondToDo = await context.AddEntityAndSaveAsync(new ToDo
        {
            Name = "Second",
            TenantId = TENANT_ID,
        });

        var thirdToDo = await context.AddEntityAndSaveAsync(new ToDo
        {
            Name = "Third",
            TenantId = TENANT_ID,
        });

        var getToDosQuery = new GetToDosQuery(context);

        var toDos = await getToDosQuery.GetAsync();

        Assert.Equal(thirdToDo.Name, toDos[0].Name);
        Assert.Equal(secondToDo.Name, toDos[1].Name);
        Assert.Equal(firstToDo.Name, toDos[2].Name);
    }
}
