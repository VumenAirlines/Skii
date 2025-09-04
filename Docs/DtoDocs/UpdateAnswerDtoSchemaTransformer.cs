using Skii.DTOs;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.OpenApi;

namespace Skii.Docs.DTO_docs;

public class UpdateAnswerDtoSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (context.JsonTypeInfo.Type != typeof(UpdateAnswerDto)) return Task.CompletedTask;
        schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
        {
            ["length_min"] = new Microsoft.OpenApi.Any.OpenApiInteger(4),
            ["length_max"] = new Microsoft.OpenApi.Any.OpenApiInteger(6),
            ["first_day_skiing"] = new Microsoft.OpenApi.Any.OpenApiBoolean(true),
            ["prefer_weekends"] = new Microsoft.OpenApi.Any.OpenApiBoolean(false),
            ["availability"] = new Microsoft.OpenApi.Any.OpenApiArray
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

          
        if (schema.Properties.TryGetValue("length_min", out var property))
            property.Description = "Optional minimum length of the ski session";

        if (schema.Properties.TryGetValue("length_max", out var schemaProperty))
            schemaProperty.Description = "Optional maximum length of the ski session";

        if (schema.Properties.TryGetValue("first_day_skiing", out var property1))
            property1.Description = "Optional flag indicating if skiing is planned for the first day";

        if (schema.Properties.TryGetValue("prefer_weekends", out var schemaProperty1))
            schemaProperty1.Description = "Optional flag indicating if the user prefers weekends";

        if (schema.Properties.ContainsKey("availability"))
            schema.Properties["availability"].Description = "Optional dictionary of available dates and corresponding choices";

        return Task.CompletedTask;
    }
}
