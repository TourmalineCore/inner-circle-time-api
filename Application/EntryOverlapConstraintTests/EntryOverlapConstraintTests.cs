using Application.EntryOverlapConstraintTests.EntryFactories;
using Core;
using Core.Entities;
using Xunit;

namespace Application.EntryOverlapConstraintTests;

[IntegrationTest]
public class EntryOverlapConstraintTests : IntegrationTestBase
{
    // Pairs that may overlap, this list needs to be expanded as new pairs appear that may overlap.
    private static readonly HashSet<(EntryType, EntryType)> _allowedOverlaps = new HashSet<(EntryType, EntryType)>
    {
        (EntryType.Task, EntryType.MakeUpTime),
    };

    private static readonly IClaimsProvider _mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

    private static readonly IReadOnlyList<EntryType> entryTypesWithoutUnspecified = Enum.GetValues<EntryType>()
        .Where(x => x != EntryType.Unspecified)
        .ToList();

    public static IEnumerable<object[]> OverlapTestDataForCreate()
    {
        foreach (EntryType entryTypeToSaveInDb in entryTypesWithoutUnspecified)
        {
            var entryFactoryToSaveInDb = EntryOverlapFactoryTestsRelated.Create(entryTypeToSaveInDb);

            foreach (EntryType entryTypeToCheckOverlap in entryTypesWithoutUnspecified)
            {
                var entryFactoryToCheckOverlap = EntryOverlapFactoryTestsRelated.Create(entryTypeToCheckOverlap);

                yield return new object[]
                {
                    entryFactoryToSaveInDb.CreateEntry(EntryOverlapFactoryTestsRelated.createTestStartTime, EntryOverlapFactoryTestsRelated.createTestEndTime),
                    entryFactoryToCheckOverlap.CreateEntryCommand(),
                    IsOverlapAllowed(entryTypeToSaveInDb, entryTypeToCheckOverlap)
                };
            }
        }
    }

    [Theory]
    [MemberData(nameof(OverlapTestDataForCreate))]
    public async Task CreateEntryAsync_ShouldRespectOverlapConstraint(
        TrackedEntryBase entryToSaveInDb,
        Func<TenantAppDbContext, IClaimsProvider, Task> createCommand,
        bool canOverlap
    )
    {
        var context = CreateTenantDbContext();

        await SaveEntityAsync(context, entryToSaveInDb);

        if (!canOverlap)
        {
            var exception = await Assert.ThrowsAsync<ConflictingTimeRangeException>(
                async () => await createCommand(context, _mockClaimsProvider)
            );

            Assert.Equal("Another task is scheduled for this time", exception.Message);
        }
        else
        {
            var exception = await Record.ExceptionAsync(
                async () => await createCommand(context, _mockClaimsProvider)
            );

            Assert.Null(exception);
        }
    }

    public static IEnumerable<object[]> OverlapTestDataForUpdate()
    {
        foreach (EntryType entryTypeToSaveInDb in entryTypesWithoutUnspecified)
        {
            var entryFactoryToSaveInDb = EntryOverlapFactoryTestsRelated.Create(entryTypeToSaveInDb);

            foreach (EntryType entryTypeToCheckOverlap in entryTypesWithoutUnspecified)
            {
                var entryFactoryToCheckOverlap = EntryOverlapFactoryTestsRelated.Create(entryTypeToCheckOverlap);

                yield return new object[]
                {
                    entryFactoryToSaveInDb.CreateEntry(EntryOverlapFactoryTestsRelated.updateTestStartTime, EntryOverlapFactoryTestsRelated.updateTestEndTime),
                    entryFactoryToCheckOverlap.CreateEntry(new DateTime(2026, 10, 23, 9, 0, 0), new DateTime(2026, 10, 23, 11, 0, 0)),
                    entryFactoryToCheckOverlap.UpdateEntryCommand(),
                    IsOverlapAllowed(entryTypeToSaveInDb, entryTypeToCheckOverlap)
                };
            }
        }
    }

    [Theory]
    [MemberData(nameof(OverlapTestDataForUpdate))]
    public async Task UpdateEntryAsync_ShouldRespectOverlapConstraint(
        TrackedEntryBase entryToSaveInDb,
        TrackedEntryBase entryToUpdate,
        Func<TenantAppDbContext, IClaimsProvider, long, Task> updateCommand,
        bool canOverlap
    )
    {
        var context = CreateTenantDbContext();

        await SaveEntityAsync(context, entryToSaveInDb);

        var entryIdToUpdate = (await SaveEntityAsync(context, entryToUpdate)).Id;

        if (!canOverlap)
        {
            var exception = await Assert.ThrowsAsync<ConflictingTimeRangeException>(
                async () => await updateCommand(context, _mockClaimsProvider, entryIdToUpdate)
            );

            Assert.Equal("Another task is scheduled for this time", exception.Message);
        }
        else
        {
            var exception = await Record.ExceptionAsync(
                async () => await updateCommand(context, _mockClaimsProvider, entryIdToUpdate)
            );

            Assert.Null(exception);
        }
    }

    private static bool IsOverlapAllowed(EntryType entryTypeToSaveInDb, EntryType entryTypeToCheckOverlap)
    {
        var pair = (entryTypeToSaveInDb, entryTypeToCheckOverlap);
        var reversedPair = (entryTypeToCheckOverlap, entryTypeToSaveInDb);

        return _allowedOverlaps.Contains(pair) || _allowedOverlaps.Contains(reversedPair);
    }
}
