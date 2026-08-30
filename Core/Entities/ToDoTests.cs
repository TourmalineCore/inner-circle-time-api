using Moq;
using Xunit;

namespace Core.Entities;

[UnitTest]
public class ToDoTests
{
    [Theory]
    [MemberData(nameof(Data))]
    public async Task GetStatus(DateTime utcNow, ToDoStatus expectedStatus)
    {
        var toDoCreatedAtUtc = new DateTime(2026, 09, 01, 14, 30, 05, 356, DateTimeKind.Utc);

        var toDo = new ToDo
        {
            Name = "First",
            CreatedAtUtc = toDoCreatedAtUtc,
        };

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        dateTimeProviderMock
            .Setup(x => x.UtcNow)
            .Returns(utcNow);

        var status = toDo.GetStatus(dateTimeProviderMock.Object);

        Assert.Equal(expectedStatus, status);
    }

    public static IEnumerable<object[]> Data =>
        new List<object[]>
        {
            new object[] { new DateTime(2026, 09, 06, 14, 30, 05, 356, DateTimeKind.Utc), ToDoStatus.New },
            new object[] { new DateTime(2026, 09, 09, 14, 30, 05, 356, DateTimeKind.Utc), ToDoStatus.Old },
            new object[] { new DateTime(2026, 09, 30, 14, 30, 05, 356, DateTimeKind.Utc), ToDoStatus.Forgotten },
        };
}
