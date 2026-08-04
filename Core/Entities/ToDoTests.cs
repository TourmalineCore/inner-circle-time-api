using Moq;
using Xunit;

namespace Core.Entities;

[UnitTest]
public class ToDoTests
{
    private const long TENANT_ID = 777;

    [Fact]
    public async Task GetStatus_FiveDaysAgoIsTreatedAsNew()
    {
        var toDoCreatedAtUtc = new DateTime(2026, 09, 24, 14, 30, 05, 356, DateTimeKind.Utc);

        var toDoThatWasCreatedFiveDaysAgo = new ToDo
        {
            Name = "First",
            TenantId = TENANT_ID,
            CreatedAtUtc = toDoCreatedAtUtc,
        };

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        dateTimeProviderMock
            .Setup(x => x.UtcNow)
            .Returns(toDoCreatedAtUtc.AddDays(5));

        var status = toDoThatWasCreatedFiveDaysAgo.GetStatus(dateTimeProviderMock.Object);

        Assert.Equal(ToDoStatus.New, status);
    }
}
