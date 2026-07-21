using Core.Entities;

namespace Application.EntryOverlapConstraintTests.EntryFactories;

public abstract class EntryOverlapFactoryTestsRelated
{
    protected static readonly long employeeId = 1;
    protected static readonly long tenantId = 777;

    public static readonly DateTime createTestStartTime = new DateTime(2026, 11, 24, 9, 0, 0);
    public static readonly DateTime createTestEndTime = new DateTime(2026, 11, 24, 11, 0, 0);
    public static readonly DateTime updateTestStartTime = new DateTime(2026, 10, 24, 9, 0, 0);
    public static readonly DateTime updateTestEndTime = new DateTime(2026, 10, 24, 11, 0, 0);

    public abstract TrackedEntryBase CreateEntry(DateTime startTime, DateTime endTime);
    public abstract Func<TenantAppDbContext, IClaimsProvider, Task> CreateEntryCommand();
    public abstract Func<TenantAppDbContext, IClaimsProvider, long, Task> UpdateEntryCommand();

    public static EntryOverlapFactoryTestsRelated Create(EntryType entryType)
    {
        return entryType switch
        {
            EntryType.Task => new TaskEntryFactoryTestsRelated(),
            EntryType.Unwell => new UnwellEntryFactoryTestsRelated(),
            EntryType.AwayWithMakeUpTime => new AwayWithMakeUpTimeEntryFactoryTestsRelated(),
            EntryType.MakeUpTime => new MakeUpTimeEntryFactoryTestsRelated(),
            EntryType.SickLeave => new SickLeaveEntryFactoryTestsRelated(),
            _ => throw new Exception($"The test is not configured to work with {entryType}.")
        };
    }
}
