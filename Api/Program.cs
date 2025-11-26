using Application;
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
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddApplication(configuration);

        var authenticationOptions = configuration.GetSection(nameof(AuthenticationOptions)).Get<AuthenticationOptions>();
        builder.Services.Configure<AuthenticationOptions>(configuration.GetSection(nameof(AuthenticationOptions)));
        builder.Services.AddJwtAuthentication(authenticationOptions).WithUserClaimsProvider<UserClaimsProvider>(UserClaimsProvider.PermissionClaimType);

        var app = builder.Build();

        app.MapOpenApi("api/swagger/openapi/v1.json");

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("openapi/v1.json", "My API V1");
            options.RoutePrefix = "api/swagger";
        });

        using (var serviceScope = app.Services.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.Migrate();
        }

        app.MapControllers();

        app.Run();
    }
}
