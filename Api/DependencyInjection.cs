using Api.ExternalDeps.AssignmentsApi;
using Application;
using Application.ExternalDeps.AssignmentsApi;
using Application.Features.Tracking.CreateTaskEntry;
using Application.Features.Tracking.CreateUnwellEntry;
using Application.Features.Tracking.GetEntriesByPeriod;
using Application.Features.Tracking.UpdateTaskEntry;
using Application.Features.Tracking.UpdateUnwellEntry;
using Application.SharedCommands;
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
        services.AddTransient<CreateTaskEntryHandler>();
        services.AddTransient<CreateTaskEntryCommand>();
        services.AddTransient<CreateUnwellEntryHandler>();
        services.AddTransient<CreateUnwellEntryCommand>();
        services.AddTransient<GetEntriesByPeriodHandler>();
        services.AddTransient<GetEntriesByPeriodQuery>();
        services.AddTransient<UpdateTaskEntryHandler>();
        services.AddTransient<UpdateTaskEntryCommand>();
        services.AddTransient<UpdateUnwellEntryHandler>();
        services.AddTransient<UpdateUnwellEntryCommand>();
        services.AddTransient<HardDeleteEntityCommand>();
    }
}
