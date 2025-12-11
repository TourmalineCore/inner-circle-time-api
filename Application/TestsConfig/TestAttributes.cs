using Xunit.Abstractions;
using Xunit.Sdk;

namespace Application.TestsConfig;

public class IntegrationTestDiscoverer : ITraitDiscoverer
{
    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        yield return new KeyValuePair<string, string>("Type", "Integration");
    }
}

public class UnitTestDiscoverer : ITraitDiscoverer
{
    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        yield return new KeyValuePair<string, string>("Type", "Unit");
    }
}

[TraitDiscoverer("Application.TestsConfig.IntegrationTestDiscoverer", "Application")]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class IntegrationTestAttribute : Attribute, ITraitAttribute
{
}

[TraitDiscoverer("Application.TestsConfig.UnitTestDiscoverer", "Application")]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class UnitTestAttribute : Attribute, ITraitAttribute
{
}
