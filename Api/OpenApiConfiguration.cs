using Microsoft.AspNetCore.Mvc.Controllers;

namespace Api;

public static class OpenApiConfiguration
{
    public static void AddConfiguredOpenApi(this IServiceCollection services)
    {
        var apiVersionFilePath = File.Exists("__version")
            // when run in docker
            ? "__version"
            // when run in IDE
            : "../__version";

        var apiVersion = File
            .ReadLines(apiVersionFilePath)
            .First();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi(options =>
        {

            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new()
                {
                    Title = "inner-circle-time-api",
                    Version = apiVersion
                };

                return Task.CompletedTask;
            });
        });
    }

    public static void AddOpenApiSchemaAndUI(this WebApplication app)
    {
        app.MapOpenApi("swagger/openapi.json");

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("openapi.json", "API");
            options.RoutePrefix = "swagger";
        });
    }
}
