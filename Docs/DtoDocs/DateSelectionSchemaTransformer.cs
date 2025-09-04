using Skii.Models;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.OpenApi;

namespace Skii.Docs.DTO_docs;

public class DateSelectionSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (context.JsonTypeInfo.Type != typeof(DateSelection)) 
            return Task.CompletedTask;

        schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
        {
            ["id"] = new Microsoft.OpenApi.Any.OpenApiString("123e4567-e89b-12d3-a456-426614174000"),
            ["answerId"] = new Microsoft.OpenApi.Any.OpenApiString("987e6543-e21b-45d3-b456-426614174111"),
            ["date"] = new Microsoft.OpenApi.Any.OpenApiString("2026-01-01"),
            ["choice"] = new Microsoft.OpenApi.Any.OpenApiInteger(2)
        };

        if (schema.Properties.TryGetValue("id", out var property))
            property.Description = "Unique identifier of the date selection";

        if (schema.Properties.TryGetValue("answerId", out var answerIdProperty))
            answerIdProperty.Description = "Identifier of the associated answer";

        if (schema.Properties.TryGetValue("date", out var dateProperty))
            dateProperty.Description = "The specific date selected by the user";

        if (schema.Properties.TryGetValue("choice", out var choiceProperty))
            choiceProperty.Description = "User's choice for the date (e.g., Available, Unavailable, Maybe)";

        schema.Properties.Remove("answer");
        
        return Task.CompletedTask;
    }
}