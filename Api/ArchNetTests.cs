using System.Reflection;
using Application;
using ArchUnitNET.Domain;
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
        .ResideInNamespace("Application")
        .As("Application Layer");

    private static readonly IObjectProvider<IType> ApiLayer = Types()
        .That()
        .ResideInNamespace("Api")
        .As("Api Layer");

    [Theory]
    [InlineData("Core", "Application")]
    [InlineData("Core", "Api")]
    [InlineData("Application", "Api")]
    public void Layer_ShouldNotHaveDependencyOnOtherLayer(string from, string to)
    {
        var fromLayer = GetLayerByName(from);
        var toLayer = GetLayerByName(to);

        IArchRule rules = Types()
            .That()
            .Are(fromLayer)
            .Should()
            .NotDependOnAny(toLayer);

        rules.Check(Architecture);
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
            $"These handlers do not have a HandleAsync method: {string.Join(", ", violations)}");
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
