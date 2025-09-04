using Skii.DTOs;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.OpenApi;

namespace Skii.Docs.DTO_docs;

public class UpdateAnswerParsedDtoSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (context.JsonTypeInfo.Type != typeof(UpdateAnswerParsedDto)) return Task.CompletedTask;
        schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
        {
            ["minLength"] = new Microsoft.OpenApi.Any.OpenApiInteger(4),
            ["maxLength"] = new Microsoft.OpenApi.Any.OpenApiInteger(6),
            ["skiiOnFirstDay"] = new Microsoft.OpenApi.Any.OpenApiBoolean(true),
            ["preferWeekends"] = new Microsoft.OpenApi.Any.OpenApiBoolean(false),
            ["availableDates"] = new Microsoft.OpenApi.Any.OpenApiArray
            {
                new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["date"] = new Microsoft.OpenApi.Any.OpenApiString("2026-01-01"),
                    ["choice"] = new Microsoft.OpenApi.Any.OpenApiInteger(2)
                },
                new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["date"] = new Microsoft.OpenApi.Any.OpenApiString("2026-01-02"),
                    ["choice"] = new Microsoft.OpenApi.Any.OpenApiInteger(1)
                }
            }
        };

          
        if (schema.Properties.TryGetValue("minLength", out var property2))
            property2.Description = "Optional minimum length of the ski session";

        if (schema.Properties.TryGetValue("maxLength", out var schemaProperty1))
            schemaProperty1.Description = "Optional maximum length of the ski session";

        if (schema.Properties.TryGetValue("skiiOnFirstDay", out var property1))
            property1.Description = "Optional flag indicating if skiing is planned for the first day";

        if (schema.Properties.TryGetValue("preferWeekends", out var schemaProperty))
            schemaProperty.Description = "Optional flag indicating if the user prefers weekends";

        if (schema.Properties.TryGetValue("availableDates", out var property))
            property.Description = "Optional list of DateSelection objects representing available dates and choices";

        return Task.CompletedTask;
    }
}
