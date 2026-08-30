using Core;
using Core.Entities;
using Xunit;

namespace Application.Features.ToDos.Handlers.GetToDos;

[UnitTest]
public class GetToDosQueryTests
{
    [Fact]
    public async Task GetAsync_ShouldReturnToDosOrderedThatNewerAreFirst()
    {
        var context = AppDbContextExtensionsTestsRelated.CreateInMemoryContextForTests();

        var firstToDo = await context.AddEntityAndSaveAsync(new ToDo
        {
            Name = "First",
            CreatedAtUtc = new DateTime(2026, 09, 01, 14, 30, 05, 356, DateTimeKind.Utc),
        });

        var secondToDo = await context.AddEntityAndSaveAsync(new ToDo
        {
            Name = "Second",
            CreatedAtUtc = new DateTime(2026, 09, 01, 15, 30, 05, 356, DateTimeKind.Utc),
        });

        var thirdToDo = await context.AddEntityAndSaveAsync(new ToDo
        {
            Name = "Third",
            CreatedAtUtc = new DateTime(2026, 09, 01, 16, 30, 05, 356, DateTimeKind.Utc),
        });

        var getToDosQuery = new GetToDosQuery(context);

        var toDos = await getToDosQuery.GetAsync();

        Assert.Equal(thirdToDo.Name, toDos[0].Name);
        Assert.Equal(secondToDo.Name, toDos[1].Name);
        Assert.Equal(firstToDo.Name, toDos[2].Name);
    }
}
