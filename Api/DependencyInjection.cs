using Api.ExternalDeps.AssignmentsApi;
using Api.ExternalDeps.EmployeesApi;
using Application;
using Application.ExternalDeps.AssignmentsApi;
using Application.ExternalDeps.EmployeesApi;
using Application.Features.Internal.GetAllProjects;
using Application.Features.Internal.GetEmployeesTrackedTaskHours;
using Application.Features.Reporting.GetAllEmployees;
using Application.Features.Reporting.GetPersonalReport;
using Application.Features.Tracking.CreateTaskEntry;
using Application.Features.Tracking.CreateUnwellEntry;
using Application.Features.Tracking.GetEntriesByPeriod;
using Application.Features.Tracking.HardDeleteEntry;
using Application.Features.Tracking.SoftDeleteEntry;
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
        services.Configure<ExternalDepsUrls>(configuration.GetSection(nameof(ExternalDepsUrls)));
        services.AddTransient<IAssignmentsApi, AssignmentsApi>();
        services.AddTransient<IEmployeesApi, EmployeesApi>();
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
        services.AddTransient<HardDeleteEntryHandler>();
        services.AddTransient<SoftDeleteEntryHandler>();
        services.AddTransient<SoftDeleteEntryCommand>();
        services.AddTransient<GetEmployeesTrackedTaskHoursHandler>();
        services.AddTransient<GetTaskEntriesQuery>();
        services.AddTransient<GetAllProjectsHandler>();
        services.AddTransient<GetAllEmployeesHandler>();
        services.AddTransient<GetPersonalReportHandler>();
        services.AddTransient<GetEmployeeTrackedEntriesQuery>();
    }
}
