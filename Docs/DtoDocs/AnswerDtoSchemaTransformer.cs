using Skii.DTOs;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.OpenApi;

namespace Skii.Docs.DTO_docs;

public class AnswerDtoSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (context.JsonTypeInfo.Type != typeof(AnswerDto)) return Task.CompletedTask;
        schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
        {
            ["id"] = new Microsoft.OpenApi.Any.OpenApiString("123e4567-e89b-12d3-a456-426614174000"),
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

           
        if (schema.Properties.TryGetValue("id", out var property))
            property.Description = "Unique identifier of the answer";

        if (schema.Properties.TryGetValue("minLength", out var schemaProperty))
            schemaProperty.Description = "Minimum length of the ski session";

        if (schema.Properties.TryGetValue("maxLength", out var property1))
            property1.Description = "Maximum length of the ski session";

        if (schema.Properties.TryGetValue("skiiOnFirstDay", out var schemaProperty1))
            schemaProperty1.Description = "Indicates if skiing is planned for the first day";

        if (schema.Properties.TryGetValue("preferWeekends", out var property2))
            property2.Description = "Indicates if the user prefers weekends";

        if (schema.Properties.TryGetValue("availableDates", out var schemaProperty2))
            schemaProperty2.Description = "List of date selections for the answer, including date and choice";

        return Task.CompletedTask;
    }
}
