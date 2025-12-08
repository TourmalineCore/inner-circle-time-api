using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Commands;

public class HardDeleteEntityCommandTests
{
    private readonly HardDeleteEntityCommand _command;
    private readonly TenantAppDbContext _context;

    public HardDeleteEntityCommandTests()
    {
        _context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests();
        _command = new HardDeleteEntityCommand(_context);
    }

    [Fact]
    public async Task DeleteExistingEntityTwice_ShouldDeleteEntityFromDbSetAndDoNotThrowAtSecondTime()
    {
        await _context.AddEntityAndSaveAsync(new WorkEntry
        {
            Id = 1
        });

        var wasDeleted = await _command.ExecuteAsync<WorkEntry>(1);

        var workEntryDoesNotExist = await _context
            .WorkEntries
            .AllAsync(x => x.Id != 1);

        Assert.True(wasDeleted);
        Assert.True(workEntryDoesNotExist);

        var wasDeletedAgain = true;

        // try to delete again
        Assert.Null(await Record.ExceptionAsync(async () => wasDeletedAgain = await _command.ExecuteAsync<WorkEntry>(1)));
        Assert.False(wasDeletedAgain);
    }

    [Fact]
    public async Task DeleteNonExistingEntity_ShouldNotThrowException()
    {
        await _context.AddEntityAndSaveAsync(new WorkEntry
        {
            Id = 1
        });

        var wasNonExistedDeleted = true;

        // try to delete a non-existent item
        Assert.Null(await Record.ExceptionAsync(async () => wasNonExistedDeleted = await _command.ExecuteAsync<WorkEntry>(2)));
        Assert.False(wasNonExistedDeleted);
    }
}
