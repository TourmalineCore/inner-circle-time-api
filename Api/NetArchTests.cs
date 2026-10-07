using System.Reflection;
using NetArchTest.Rules;
using Xunit;

namespace Api;

public class NetArchTests
{
    [Theory]
    [InlineData("Core", "Application")]
    [InlineData("Core", "Api")]
    [InlineData("Application", "Api")]
    public void Layer_ShouldNotHaveDependencyOnOtherLayer(string from, string to)
    {
        var fromAssembly = Assembly.Load(from);
        var toAssembly = Assembly.Load(to);

        var result = Types
            .InAssembly(fromAssembly)
            .Should()
            .NotHaveDependencyOn(toAssembly.GetName().Name)
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            $"{from} must not depend on the {to}. " +
            $"Violators: {string.Join(", ", result.FailingTypeNames ?? [])}"
        );
    }

    [Fact]
    public void AllHandlers_ShouldHaveHandleAsyncMethod()
    {
        var handlerTypes = Types
            .InAssembly(Assembly.Load("Application"))
            .That()
            .HaveNameEndingWith("Handler")
            .GetTypes();

        var violations = new List<string>();

        foreach (var type in handlerTypes)
        {
            var method = type.GetMethod("HandleAsync",
                BindingFlags.Public | BindingFlags.Instance);

            if (method == null)
            {
                violations.Add(type.FullName);
            }
        }

        Assert.True(violations.Count == 0,
            $"These handlers do not have a HandleAsync method: {string.Join(", ", violations)}");
    }
}
