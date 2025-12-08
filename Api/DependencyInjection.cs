using Api.Features.Tracking.CreateWorkEntry;
using Api.Features.Tracking.GetWorkEntriesByPeriod;
using Application;
using Application.Commands;
using Application.Queries;
using Microsoft.EntityFrameworkCore;

namespace Api;

public static class DependencyInjection
{
    private const string DefaultConnection = "DefaultConnection";

    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // https://stackoverflow.com/a/37373557
        services.AddHttpContextAccessor();
        services.AddScoped<IClaimsProvider, HttpContextClaimsProvider>();

        var connectionString = configuration.GetConnectionString(DefaultConnection);

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<TenantAppDbContext>();

        services.AddTransient<CreateWorkEntryHandler>();
        services.AddTransient<CreateWorkEntryCommand>();
        services.AddTransient<GetWorkEntriesByPeriodHandler>();
        services.AddTransient<GetWorkEntriesByPeriodQuery>();
        services.AddTransient<HardDeleteEntityCommand>();
        services.AddTransient<HardDeleteWorkEntryCommand>();
    }
}
