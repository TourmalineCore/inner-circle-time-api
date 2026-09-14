using Api.ExternalDeps.AssignmentsApi;
using Api.ExternalDeps.EmployeesApi;
using Application;
using Application.ExternalDeps.AssignmentsApi;
using Application.ExternalDeps.EmployeesApi;
using Application.Features.Internal.Handlers.GetAllProjects;
using Application.Features.Internal.Handlers.GetEmployeesTrackedTaskHours;
using Application.Features.Reporting.Handlers.GetAllEmployees;
using Application.Features.Reporting.Handlers.GetPersonalReport;
using Application.Features.Tracking.Handlers.CreateAwayWithMakeUpTimeEntry;
using Application.Features.Tracking.Handlers.CreateSickLeaveEntry;
using Application.Features.Tracking.Handlers.CreateTaskEntry;
using Application.Features.Tracking.Handlers.CreateUnwellEntry;
using Application.Features.Tracking.Handlers.CreateVacationEntry;
using Application.Features.Tracking.Handlers.GetAwayWithMakeUpTimeEntry;
using Application.Features.Tracking.Handlers.GetEntriesByPeriod;
using Application.Features.Tracking.Handlers.GetSickLeaveEntry;
using Application.Features.Tracking.Handlers.GetTaskEntry;
using Application.Features.Tracking.Handlers.GetUnwellEntry;
using Application.Features.Tracking.Handlers.GetVacationEntry;
using Application.Features.Tracking.Handlers.HardDeleteEntry;
using Application.Features.Tracking.Handlers.SoftDeleteEntry;
using Application.Features.Tracking.Handlers.UpdateAwayWithMakeUpTimeEntry;
using Application.Features.Tracking.Handlers.UpdateSickLeaveEntry;
using Application.Features.Tracking.Handlers.UpdateTaskEntry;
using Application.Features.Tracking.Handlers.UpdateUnwellEntry;
using Application.Features.Tracking.Handlers.UpdateVacationEntry;
using Application.SharedCommands;
using Application.SharedQueries;
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
        services.AddTransient<CreateAwayWithMakeUpTimeEntryHandler>();
        services.AddTransient<CreateAwayWithMakeUpTimeEntryCommand>();
        services.AddTransient<CreateSickLeaveEntryHandler>();
        services.AddTransient<CreateSickLeaveEntryCommand>();
        services.AddTransient<CreateVacationEntryHandler>();
        services.AddTransient<CreateVacationEntryCommand>();
        services.AddTransient<GetTaskEntryHandler>();
        services.AddTransient<GetUnwellEntryHandler>();
        services.AddTransient<GetAwayWithMakeUpTimeEntryHandler>();
        services.AddTransient<GetEntriesByPeriodHandler>();
        services.AddTransient<GetSickLeaveEntryHandler>();
        services.AddTransient<GetVacationEntryHandler>();
        services.AddTransient<GetEntriesByPeriodQuery>();
        services.AddTransient<UpdateTaskEntryHandler>();
        services.AddTransient<UpdateTaskEntryCommand>();
        services.AddTransient<UpdateUnwellEntryHandler>();
        services.AddTransient<UpdateUnwellEntryCommand>();
        services.AddTransient<UpdateAwayWithMakeUpTimeEntryHandler>();
        services.AddTransient<UpdateAwayWithMakeUpTimeEntryCommand>();
        services.AddTransient<UpdateSickLeaveEntryHandler>();
        services.AddTransient<UpdateSickLeaveEntryCommand>();
        services.AddTransient<UpdateVacationEntryHandler>();
        services.AddTransient<UpdateVacationEntryCommand>();
        services.AddTransient<HardDeleteEntityCommand>();
        services.AddTransient<HardDeleteEntryHandler>();
        services.AddTransient<SoftDeleteEntryHandler>();
        services.AddTransient<SoftDeleteEntryCommand>();
        services.AddTransient<GetEmployeesTrackedTaskHoursHandler>();
        services.AddTransient<IGetTaskEntriesQuery, GetTaskEntriesQuery>();
        services.AddTransient<GetAllProjectsHandler>();
        services.AddTransient<GetAllEmployeesHandler>();
        services.AddTransient<GetPersonalReportHandler>();
        services.AddTransient<IGetEntryByIdQuery, GetEntryByIdQuery>();
        services.AddTransient<GetEmployeeTrackedEntriesQuery>();
    }
}
