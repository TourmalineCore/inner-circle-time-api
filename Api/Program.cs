using Application;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

namespace Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        // Add services to the container.
        builder.Services.AddControllers();

        builder.Services.AddConfiguredOpenApi();

        builder.Services.AddApplication(configuration);

        builder.Services.AddProblemDetails(options =>
        {
            var enableExceptionDetails = Environment.GetEnvironmentVariable("ENABLE_EXCEPTION_DETAILS");

            options.IncludeExceptionDetails = (ctx, ex) => enableExceptionDetails!.Equals("true", StringComparison.OrdinalIgnoreCase);

            options.Map<InvalidTimeRangeException>(ex =>
            {
                return new ProblemDetails
                {
                    Title = "Invalid time range",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = ex.Message,
                };
            });

            options.Map<ConflictingTimeRangeException>(ex =>
            {
                return new ProblemDetails
                {
                    Title = "Conflicting time range",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = ex.Message,
                };
            });
        });

        builder.Services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Title = "Validation error",
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "Fill in all the fields",
                        Instance = context.HttpContext.Request.Path
                    };

                    throw new ProblemDetailsException(problemDetails);
                };
            });

        var authenticationOptions = configuration.GetSection(nameof(AuthenticationOptions)).Get<AuthenticationOptions>();
        builder.Services.Configure<AuthenticationOptions>(configuration.GetSection(nameof(AuthenticationOptions)));
        builder.Services.AddJwtAuthentication(authenticationOptions).WithUserClaimsProvider<UserClaimsProvider>(UserClaimsProvider.PermissionClaimType);

        var app = builder.Build();

        app.UseProblemDetails();

        app.AddOpenApiSchemaAndUI();

        MigrateDatabase(app.Services);

        app.MapControllers();

        app.Run();
    }

    private static void MigrateDatabase(IServiceProvider serviceProvider)
    {
        using var serviceScope = serviceProvider.CreateScope();

        var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }
}
