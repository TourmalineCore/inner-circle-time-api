using System.Reflection;
using Application;
using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using Core;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Api;

[UnitTest]
public class ArchNetTests
{
    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            System.Reflection.Assembly.Load("Core"),
            System.Reflection.Assembly.Load("Application"),
            System.Reflection.Assembly.Load("Api")
        )
        .Build();

    private static readonly IObjectProvider<IType> CoreLayer = Types()
        .That()
        .ResideInNamespace("Core")
        .As("Core Layer");

    private static readonly IObjectProvider<IType> ApplicationLayer = Types()
        .That()
        .ResideInAssembly(typeof(ApplicationAssemblyMarker).Assembly)
        .As("Application Layer");

    private static readonly IObjectProvider<IType> ApiLayer = Types()
        .That()
        .ResideInAssembly(typeof(ApplicationAssemblyMarker).Assembly)
        .As("Api Layer");

    [Theory]
    [InlineData("Core", "Application")]
    [InlineData("Core", "Api")]
    [InlineData("Application", "Api")]
    public void Layer_ShouldNotHaveDependencyOnOtherLayer(string from, string to)
    {
        var fromLayer = GetLayerByName(from);
        var toLayer = GetLayerByName(to);

        IArchRule rule = Types()
            .That()
            .Are(fromLayer)
            .Should()
            .NotDependOnAny(toLayer);

        rule.Check(Architecture);
    }

    [Fact]
    public void AllHandlers_ShouldHaveHandleAsyncMethod()
    {
        var handlers = Types()
            .That()
            .ResideInAssembly(typeof(ApplicationAssemblyMarker).Assembly)
            .And()
            .HaveNameEndingWith("Handler")
            .GetObjects(Architecture);

        var violations = new List<string>();

        foreach (var handler in handlers)
        {
            var hasMethod = handler.Members
                .OfType<MethodMember>()
                .Any(x => x.Name.StartsWith("HandleAsync", StringComparison.OrdinalIgnoreCase));

            if (!hasMethod)
            {
                violations.Add(handler.FullName);
            }
        }

        Assert.True(violations.Count == 0,
            $"These handlers do not have a HandleAsync method: {string.Join(", ", violations)}"
        );
    }

    [Fact]
    public void Controllers_ShouldOnlyCallHandlerMethods()
    {
        var controllers = Classes()
            .That()
            .ResideInAssembly(typeof(ApiAssemblyMarker).Assembly)
            .And()
            .HaveNameEndingWith("Controller")
            .GetObjects(Architecture);

        var violations = new List<string>();

        foreach (var controller in controllers)
        {
            foreach (var method in controller.GetMethodMembers())
            {
                foreach (var call in method.GetCalledMethods())
                {
                    var isHandler = call.Name.StartsWith(
                        "HandleAsync",
                        StringComparison.OrdinalIgnoreCase);

                    // Skip system and framework calls
                    var declaringType = call.DeclaringType?.FullName ?? string.Empty;

                    var isSystemCall =
                        declaringType.StartsWith("Microsoft.") ||
                        declaringType.StartsWith("System.") ||
                        declaringType.StartsWith("Api.") ||
                        call.Name == ".ctor" ||
                        call.Name == ".cctor";

                    if (isSystemCall)
                    {
                        continue;
                    }

                    if (!isHandler)
                    {
                        violations.Add(
                            $"  — {controller.Name}.{method.Name}() " +
                            $"calls {declaringType}::{call.Name}() " +
                            $"— controllers may only call HandleAsync() methods"
                        );
                    }
                }
            }
        }

        Assert.True(violations.Count == 0,
            $"Controllers must only call HandleAsync() methods from the Application layer. {Environment.NewLine}" +
            $"Found {violations.Count} forbidden call(s):{Environment.NewLine}" +
            $"{string.Join(Environment.NewLine, violations)}"
        );
    }

    private static IObjectProvider<IType> GetLayerByName(string name)
    {
        return name switch
        {
            "Core" => CoreLayer,
            "Application" => ApplicationLayer,
            "Api" => ApiLayer,
            _ => throw new NotImplementedException(),
        };
    }
}
