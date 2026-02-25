using System.Security.Claims;
using System.Text.Encodings.Web;
using Api;
using Application;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public class HttpClientTestBase : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    protected const long EMPLOYEE_ID = 1;
    protected const long TENANT_ID = 777;

    protected HttpClient HttpClient = null!;
    private WebApplicationFactory<Program> _factory = null!;

    public HttpClientTestBase(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                // Replacing the authentication service with a fake
                services
                    .AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>("Test", options => { });

                // Add fake mockClaimsProvider
                var mockClaimsProvider = new Mock<IClaimsProvider>();
                mockClaimsProvider
                    .Setup(x => x.EmployeeId)
                    .Returns(EMPLOYEE_ID);
                mockClaimsProvider
                    .Setup(x => x.TenantId)
                    .Returns(TENANT_ID);

                services.AddScoped(_ => mockClaimsProvider.Object);
            });
        });

        HttpClient = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        HttpClient.Dispose();
        _factory.Dispose();
    }
}

public class FakeAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public FakeAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder
    ) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            // Adding permissions claims to bypass permissions
            new Claim("permissions", UserClaimsProvider.CanManagePersonalTimeTracker),
            new Claim("permissions", UserClaimsProvider.AUTO_TESTS_ONLY_IsEntriesHardDeleteAllowed)
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
