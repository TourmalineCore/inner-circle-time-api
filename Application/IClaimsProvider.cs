namespace Application;


/// <summary>
/// we need a way to inject TenantId to the logic instead of passing it manually through all layers
/// Api implements IClaimsProvider to provide it from a JWT token per each web request (scoped)
/// actual implementation was inspired by this code snippet https://stackoverflow.com/a/75203625
/// </summary>
public interface IClaimsProvider
{
    long TenantId { get; }
}
