using Application.TestsConfig;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Commands;

[UnitTest]
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
        var newWorkEntryId = await _context.AddEntityAndSaveAsync(new WorkEntry());

        var wasDeleted = await _command.ExecuteAsync<WorkEntry>(newWorkEntryId);

        var deletedWorkEntry = await _context
            .WorkEntries
            .SingleOrDefaultAsync(x => x.Id != newWorkEntryId);

        Assert.True(wasDeleted);
        Assert.Null(deletedWorkEntry);

        var wasDeletedAgain = true;

        // try to delete again
        Assert.Null(await Record.ExceptionAsync(async () => wasDeletedAgain = await _command.ExecuteAsync<WorkEntry>(newWorkEntryId)));
        Assert.False(wasDeletedAgain);
    }

    [Fact]
    public async Task DeleteNonExistingEntity_ShouldNotThrowException()
    {
        var wasNonExistedDeleted = true;

        const long NON_EXISTING_ID = -1;

        // try to delete a non-existing item
        Assert.Null(await Record.ExceptionAsync(async () => wasNonExistedDeleted = await _command.ExecuteAsync<WorkEntry>(NON_EXISTING_ID)));
        Assert.False(wasNonExistedDeleted);
    }
}
