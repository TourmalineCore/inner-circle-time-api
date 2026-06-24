using Application.Features.Tracking.CreateAwayWithMakeUpTimeEntry;
using Application.Features.Tracking.CreateTaskEntry;
using Application.Features.Tracking.CreateUnwellEntry;
using Application.Features.Tracking.UpdateAwayWithMakeUpTimeEntry;
using Application.Features.Tracking.UpdateTaskEntry;
using Application.Features.Tracking.UpdateUnwellEntry;
using Core;
using Core.Entities;
using Xunit;

namespace Application.Features.Tracking;

public class EntryOverlapTestFactory
{
    public Func<DateTime, DateTime, TrackedEntryBase> CreateEntry { get; set; } = null!;
    public Func<Func<TenantAppDbContext, IClaimsProvider, Task>> CreateEntryCommand { get; set; } = null!;
    public Func<Func<TenantAppDbContext, IClaimsProvider, long, Task>> UpdateEntryCommand { get; set; } = null!;
}

[IntegrationTest]
public class EntryOverlapConstraintTests : IntegrationTestBase
{
    private static readonly DateTime _createTestStartTime = new DateTime(2026, 11, 24, 9, 0, 0);
    private static readonly DateTime _createTestEndTime = new DateTime(2026, 11, 24, 11, 0, 0);

    private static readonly DateTime _updateTestStartTime = new DateTime(2026, 10, 24, 9, 0, 0);
    private static readonly DateTime _updateTestEndTime = new DateTime(2026, 10, 24, 11, 0, 0);

    // Pairs that may overlap, this list needs to be expanded as new pairs appear that may overlap.
    private static readonly HashSet<(EntryType, EntryType)> _allowedOverlaps = new HashSet<(EntryType, EntryType)>
    {
        (EntryType.Task, EntryType.MakeUpTime),
    };

    private static readonly IClaimsProvider _mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

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

    public static IEnumerable<object[]> OverlapTestDataForCreate()
    {
        var entryTypesWithoutUnspecified = Enum.GetValues<EntryType>()
            .Where(x => x != EntryType.Unspecified)
            .ToList();

        foreach (EntryType entryTypeToSaveInDb in entryTypesWithoutUnspecified)
        {
            var saveFactory = CreateEntryFactory(entryTypeToSaveInDb);

            foreach (EntryType entryTypeToCheckOverlap in entryTypesWithoutUnspecified)
            {
                var entryToCheckOverlapFactory = CreateEntryFactory(entryTypeToCheckOverlap);

                yield return new object[]
                {
                    saveFactory.CreateEntry(_createTestStartTime, _createTestEndTime),
                    entryToCheckOverlapFactory.CreateEntryCommand(),
                    IsOverlapAllowed(entryTypeToSaveInDb, entryTypeToCheckOverlap)
                };
            }
        }
    }

    public static IEnumerable<object[]> OverlapTestDataForUpdate()
    {
        var entryTypesWithoutUnspecified = Enum.GetValues<EntryType>()
            .Where(x => x != EntryType.Unspecified)
            .ToArray();

        foreach (EntryType entryTypeToSaveInDb in entryTypesWithoutUnspecified)
        {
            var saveFactory = CreateEntryFactory(entryTypeToSaveInDb);

            foreach (EntryType entryTypeToCheckOverlap in entryTypesWithoutUnspecified)
            {
                var entryToCheckOverlapFactory = CreateEntryFactory(entryTypeToCheckOverlap);

                yield return new object[]
                {
                    saveFactory.CreateEntry(_updateTestStartTime, _updateTestEndTime),
                    entryToCheckOverlapFactory.CreateEntry(new DateTime(2026, 10, 23, 9, 0, 0), new DateTime(2026, 10, 23, 11, 0, 0)),
                    entryToCheckOverlapFactory.UpdateEntryCommand(),
                    IsOverlapAllowed(entryTypeToSaveInDb, entryTypeToCheckOverlap)
                };
            }
        }
    }

    private static EntryOverlapTestFactory CreateEntryFactory(EntryType entryType)
    {
        return entryType switch
        {
            EntryType.Task => new EntryOverlapTestFactory
            {
                CreateEntry = (startTime, endTime) => new TaskEntry
                {
                    EmployeeId = EMPLOYEE_ID,
                    Title = "Existing Task",
                    StartTime = startTime,
                    EndTime = endTime,
                    TaskId = "#2231",
                    ProjectId = 1,
                    Description = "Task description",
                },
                CreateEntryCommand = () =>
                    (context, claimsProvider) => new CreateTaskEntryCommand(context, claimsProvider)
                        .ExecuteAsync(new CreateTaskEntryRequest
                        {
                            Title = "New Task",
                            StartTime = _createTestStartTime,
                            EndTime = _createTestEndTime,
                            TaskId = "#222",
                            ProjectId = 1,
                            Description = "Description"
                        }),
                UpdateEntryCommand = () =>
                    (context, claimsProvider, entryId) => new UpdateTaskEntryCommand(context, claimsProvider)
                        .ExecuteAsync(new UpdateTaskEntryRequest
                        {
                            Id = entryId,
                            Title = "New Task",
                            StartTime = _updateTestStartTime,
                            EndTime = _updateTestEndTime,
                            TaskId = "#222",
                            ProjectId = 1,
                            Description = "Description"
                        })
            },
            EntryType.Unwell => new EntryOverlapTestFactory
            {
                CreateEntry = (startTime, endTime) => new UnwellEntry
                {
                    EmployeeId = EMPLOYEE_ID,
                    StartTime = startTime,
                    EndTime = endTime,
                },
                CreateEntryCommand = () =>
                    (context, claimsProvider) => new CreateUnwellEntryCommand(context, claimsProvider)
                        .ExecuteAsync(new CreateUnwellEntryRequest
                        {
                            StartTime = _createTestStartTime,
                            EndTime = _createTestEndTime,
                        }),
                UpdateEntryCommand = () =>
                    (context, claimsProvider, entryId) => new UpdateUnwellEntryCommand(context, claimsProvider)
                        .ExecuteAsync(new UpdateUnwellEntryRequest
                        {
                            Id = entryId,
                            StartTime = _updateTestStartTime,
                            EndTime = _updateTestEndTime,
                        })
            },
            EntryType.AwayWithMakeUpTime => new EntryOverlapTestFactory
            {
                CreateEntry = (startTime, endTime) => new AwayWithMakeUpTimeEntry
                {
                    EmployeeId = EMPLOYEE_ID,
                    StartTime = startTime,
                    EndTime = endTime,
                    Description = "Description",
                    MakeUpTimeList = []
                },
                CreateEntryCommand = () =>
                    (context, claimsProvider) => new CreateAwayWithMakeUpTimeEntryCommand(context, claimsProvider)
                        .ExecuteAsync(new CreateAwayWithMakeUpTimeEntryRequest
                        {
                            StartTime = _createTestStartTime,
                            EndTime = _createTestEndTime,
                            Description = "Description",
                            MakeUpTimeList = []
                        }),
                UpdateEntryCommand = () =>
                    (context, claimsProvider, entryId) => new UpdateAwayWithMakeUpTimeEntryCommand(context, claimsProvider)
                        .ExecuteAsync(new UpdateAwayWithMakeUpTimeEntryRequest
                        {
                            Id = entryId,
                            StartTime = _updateTestStartTime,
                            EndTime = _updateTestEndTime,
                            Description = "Description",
                            MakeUpTimeList = []
                        })
            },
            // MakeUpTimeEntry requires a linked record to be saved.
            // Create it together with the "Away With Make Up Time Entry".
            EntryType.MakeUpTime => new EntryOverlapTestFactory
            {
                CreateEntry = (startTime, endTime) => new AwayWithMakeUpTimeEntry
                {
                    EmployeeId = EMPLOYEE_ID,
                    // Subtract one week to prevent any overlap.
                    StartTime = startTime.AddDays(-7),
                    EndTime = endTime.AddDays(-7),
                    Description = "Description",
                    MakeUpTimeList =
                    [
                        new MakeUpTimeEntry
                        {
                            TenantId = TENANT_ID,
                            EmployeeId = EMPLOYEE_ID,
                            StartTime = startTime,
                            EndTime = endTime
                        }
                    ]
                },
                CreateEntryCommand = () =>
                    (context, claimsProvider) => new CreateAwayWithMakeUpTimeEntryCommand(context, claimsProvider)
                        .ExecuteAsync(new CreateAwayWithMakeUpTimeEntryRequest
                        {
                            // Subtract one week to prevent any overlap.
                            StartTime = _createTestStartTime.AddDays(-7),
                            EndTime = _createTestEndTime.AddDays(-7),
                            Description = "Description",
                            MakeUpTimeList = [
                                new MakeUpTimeEntryDto
                                {
                                    StartTime = _createTestStartTime,
                                    EndTime = _createTestEndTime,
                                }
                            ]
                        }),

                UpdateEntryCommand = () =>
                    (context, claimsProvider, entryId) => new UpdateAwayWithMakeUpTimeEntryCommand(context, claimsProvider)
                        .ExecuteAsync(new UpdateAwayWithMakeUpTimeEntryRequest
                        {
                            Id = entryId,
                            // Subtract one week to prevent any overlap.
                            StartTime = _updateTestStartTime.AddDays(-7),
                            EndTime = _updateTestEndTime.AddDays(-7),
                            Description = "Description",
                            MakeUpTimeList = [
                                new MakeUpTimeEntryDto
                                {
                                    StartTime = _updateTestStartTime,
                                    EndTime = _updateTestEndTime,
                                }
                            ]
                        })
            },
            _ => throw new Exception($"The test is not configured to work with {entryType}.")
        };
    }

    private static bool IsOverlapAllowed(EntryType entryTypeToSaveInDb, EntryType entryTypeToCheckOverlap)
    {
        var pair = (entryTypeToSaveInDb, entryTypeToCheckOverlap);
        var reversedPair = (entryTypeToCheckOverlap, entryTypeToSaveInDb);

        return _allowedOverlaps.Contains(pair) || _allowedOverlaps.Contains(reversedPair);
    }
}
