using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Commands;

public class HardDeleteEntityCommandTests
{
    [Fact]
    public async Task DeleteExistingEntityTwice_ShouldDeleteEntityFromDbSetAndDoNotThrowAtSecondTime()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests();

        await context.AddEntityAndSaveAsync(new WorkEntry
        {
            Id = 1
        });

        var hardDeleteEntityCommand = new HardDeleteEntityCommand(context);

        var wasDeleted = await hardDeleteEntityCommand.ExecuteAsync<WorkEntry>(1);

        var workEntryDoesNotExist = await context
            .WorkEntries
            .AllAsync(x => x.Id != 1);

        Assert.True(wasDeleted);
        Assert.True(workEntryDoesNotExist);

        var wasDeletedAgain = true;

        // try to delete again
        Assert.Null(await Record.ExceptionAsync(async () => wasDeletedAgain = await hardDeleteEntityCommand.ExecuteAsync<WorkEntry>(1)));
        Assert.False(wasDeletedAgain);
    }

    [Fact]
    public async Task DeleteNonExistingEntity_ShouldNotThrowException()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests();

        await context.AddEntityAndSaveAsync(new WorkEntry
        {
            Id = 1
        });

        var hardDeleteEntityCommand = new HardDeleteEntityCommand(context);

        var wasNonExistedDeleted = true;

        // try to delete a non-existent item
        Assert.Null(await Record.ExceptionAsync(async () => wasNonExistedDeleted = await hardDeleteEntityCommand.ExecuteAsync<WorkEntry>(2)));
        Assert.False(wasNonExistedDeleted);
    }
}
