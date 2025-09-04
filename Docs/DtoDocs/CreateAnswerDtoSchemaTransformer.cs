using Skii.DTOs;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.OpenApi;

namespace Skii.Docs.DTO_docs;

public class CreateAnswerDtoSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (context.JsonTypeInfo.Type != typeof(CreateAnswerDto)) return Task.CompletedTask;
        schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
        {
            ["length_min"] = new Microsoft.OpenApi.Any.OpenApiInteger(4),
            ["length_max"] = new Microsoft.OpenApi.Any.OpenApiInteger(6),
            ["first_day_skiing"] = new Microsoft.OpenApi.Any.OpenApiBoolean(true),
            ["prefer_weekends"] = new Microsoft.OpenApi.Any.OpenApiBoolean(true),
            ["availability"] = new Microsoft.OpenApi.Any.OpenApiObject
            {
                ["2026-01-01"] = new Microsoft.OpenApi.Any.OpenApiInteger(2),
                ["2026-01-02"] = new Microsoft.OpenApi.Any.OpenApiInteger(1)
            }
        };

       
        if (schema.Properties.TryGetValue("length_min", out var property))
            property.Description = "Minimum length of the ski session";

        if (schema.Properties.TryGetValue("length_max", out var schemaProperty))
            schemaProperty.Description = "Maximum length of the ski session";

        if (schema.Properties.TryGetValue("first_day_skiing", out var property1))
            property1.Description = "Indicates if the user wants to ski on the first day";

        if (schema.Properties.TryGetValue("prefer_weekends", out var schemaProperty1))
            schemaProperty1.Description = "Indicates if the user prefers skiing on weekends";

        if (schema.Properties.TryGetValue("availability", out var property2))
            property2.Description = "Dictionary of dates and corresponding availability choices";

        return Task.CompletedTask;
    }
}
