using Application.TestsConfig;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Application.Commands;

[UnitTest]
public class HardDeleteEntityCommandTests
{
    protected const long EMPLOYEE_ID = 1;
    protected const long TENANT_ID = 777;

    private readonly HardDeleteEntityCommand _command;
    private readonly TenantAppDbContext _context;

    public HardDeleteEntityCommandTests()
    {
        _context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests();
        var mockClaimsProvider = new Mock<IClaimsProvider>();

        mockClaimsProvider
            .Setup(x => x.EmployeeId)
            .Returns(EMPLOYEE_ID);
        mockClaimsProvider
            .Setup(x => x.TenantId)
            .Returns(TENANT_ID);

        _command = new HardDeleteEntityCommand(_context, mockClaimsProvider.Object);
    }

    [Fact]
    public async Task DeleteExistingEntityTwice_ShouldDeleteEntityFromDbSetAndDoNotThrowAtSecondTime()
    {
        var workEntry = await _context.AddEntityAndSaveAsync(new WorkEntry
        {
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID
        });

        var wasDeleted = await _command.ExecuteAsync<WorkEntry>(workEntry.Id);

        var deletedWorkEntry = await _context
            .WorkEntries
            .SingleOrDefaultAsync(x => x.Id != workEntry.Id);

        Assert.True(wasDeleted);
        Assert.Null(deletedWorkEntry);

        var wasDeletedAgain = true;

        // try to delete again
        Assert.Null(await Record.ExceptionAsync(async () => wasDeletedAgain = await _command.ExecuteAsync<WorkEntry>(workEntry.Id)));
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