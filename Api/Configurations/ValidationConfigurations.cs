using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;

namespace Api.Configurations;

public static class ValidationConfiguration
{
    public static void ConfigureValidation(IServiceCollection services, IHostEnvironment environment)
    {
        services.AddProblemDetails(options =>
        {
            options.IncludeExceptionDetails = (ctx, ex) => environment.IsDevelopment();
        });

        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Type = "https://example.com/validation-error",
                        Title = "Validation error",
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "Fill in all the fields",
                        Instance = context.HttpContext.Request.Path
                    };

                    throw new ProblemDetailsException(problemDetails);
                };
            });
    }
}
