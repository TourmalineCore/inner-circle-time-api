using Application.TestsConfig;

namespace Application.Commands;


// It partial class used in create and update tests in order to run these tests sequentially,
// this avoids a race condition between these tests
// https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/partial-classes-and-methods
// Error example: System.InvalidOperationException : An exception has been raised that is likely due to a transient failure.
//---- Microsoft.EntityFrameworkCore.DbUpdateException : An error occurred while saving the entity changes. See the inner exception for details.
//-------- Npgsql.PostgresException : 40P01: deadlock detected
[IntegrationTest]
public partial class WorkEntryCommandTestsBase : IntegrationTestBase
{

}
