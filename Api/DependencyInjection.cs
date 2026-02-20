using Api.ExternalDeps.AssignmentsApi;
using Api.Features.Tracking.CreateUnwellEntry;
using Api.Features.Tracking.CreateWorkEntry;
using Api.Features.Tracking.GetWorkEntriesByPeriod;
using Api.Features.Tracking.UpdateUnwellEntry;
using Api.Features.Tracking.UpdateWorkEntry;
using Application;
using Application.Commands;
using Application.ExternalDeps.AssignmentsApi;
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
        services.AddTransient<IAssignmentsApi, AssignmentsApi>();
        services.AddTransient<CreateWorkEntryHandler>();
        services.AddTransient<CreateWorkEntryCommand>();
        services.AddTransient<CreateUnwellEntryHandler>();
        services.AddTransient<CreateUnwellEntryCommand>();
        services.AddTransient<GetWorkEntriesByPeriodHandler>();
        services.AddTransient<GetWorkEntriesByPeriodQuery>();
        services.AddTransient<UpdateWorkEntryHandler>();
        services.AddTransient<UpdateWorkEntryCommand>();
        services.AddTransient<UpdateUnwellEntryHandler>();
        services.AddTransient<UpdateUnwellEntryCommand>();
        services.AddTransient<HardDeleteEntityCommand>();
    }
}
