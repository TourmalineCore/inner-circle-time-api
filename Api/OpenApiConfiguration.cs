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
            options.AddOperationTransformer((operation, context, _) =>
            {
                // Try to get the ControllerActionDescriptor to access method information
                if (context.Description.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    // Set the operationId to the ControllerName and ActionName (which is typically the method name)
                    // This allows to have unique operationId even if there is the same method name across multiple controllers
                    operation.OperationId = $"{controllerActionDescriptor.ControllerName}{controllerActionDescriptor.ActionName}";
                }

                return Task.CompletedTask;
            });

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
        app.MapOpenApi("api/swagger/openapi.json");

        // Redirect request from /api/swagger/openapi/v1.json to /api/swagger/openapi.json
        app.MapGet("api/swagger/openapi/v1.json", async (HttpContext context) =>
        {
            context.Response.Redirect("/api/swagger/openapi.json");
        });

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("openapi.json", "API");
            options.RoutePrefix = "api/swagger";
        });
    }
}
