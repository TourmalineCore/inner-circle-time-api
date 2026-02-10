
// It partial class used in create and update tests in order to run these tests sequentially,
// this avoids a race condition between these tests
// https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/partial-classes-and-methods
using Application.TestsConfig;

namespace Application.Commands;

[IntegrationTest]
public partial class WorkEntryCommandTestsBase : IntegrationTestBase
{

}