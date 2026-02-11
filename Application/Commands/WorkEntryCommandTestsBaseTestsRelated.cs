using Application.TestsConfig;

namespace Application.Commands;

// This partial class was introduced to inherited by create and update commands tests in order to run these tests sequentially
// https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/partial-classes-and-methods

// Before that there were 2 separate clasees and they were running cincurrently and often lead to a deadlock and thus tests were flacky
// There is an issue to investigate the root cause why they fail https://github.com/TourmalineCore/inner-circle-time-api/issues/26
[IntegrationTest]
public partial class WorkEntryCommandTestsBase : IntegrationTestBase
{

}
