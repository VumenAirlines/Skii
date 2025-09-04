using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.OpenApi;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Skii.Docs;

public class AnswerControllerOperationTransformer : IOpenApiOperationTransformer
{
    private readonly Dictionary<string, OperationDocumentation> _documentation = new()
    {
        ["GetAnswerById"] = new OperationDocumentation
        {
            Summary = "Get a user's answer by ID",
            Description = "Fetches an existing answer for the authenticated user by its ID.",
            Parameters = new Dictionary<string, string>
            {
                ["id"] = "The unique identifier of the answer to fetch"
            },
            Responses = new Dictionary<int, string>
            {
                [200] = "Answer retrieved successfully.",
                [401] = "Unauthorized. User must be authenticated.",
                [404] = "Answer not found for the given ID.",
                [500] = "Internal server error."
            }
        },
        ["CreateAnswer"] = new OperationDocumentation
        {
            Summary = "Create a new answer",
            Description = "Creates a new answer for the authenticated user.",
            Parameters = new Dictionary<string, string>(),
            Responses = new Dictionary<int, string>
            {
                [201] = "Answer created successfully.",
                [400] = "Invalid answer data.",
                [401] = "Unauthorized. User must be authenticated.",
                [500] = "Internal server error."
            }
        },
        ["UpdateAnswer"] = new OperationDocumentation
        {
            Summary = "Update an existing answer",
            Description = "Updates an existing answer for the authenticated user by ID.",
            Parameters = new Dictionary<string, string>
            {
                ["id"] = "The ID of the answer to update"
            },
            Responses = new Dictionary<int, string>
            {
                [204] = "Answer updated successfully.",
                [400] = "Invalid update data.",
                [401] = "Unauthorized. User must be authenticated.",
                [404] = "Answer not found for the given ID.",
                [500] = "Internal server error."
            }
        }
    };

    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var actionName = context.Description.ActionDescriptor.RouteValues["action"];
        if (actionName == null || !_documentation.TryGetValue(actionName, out var doc)) return Task.CompletedTask;

       
        operation.Summary = doc.Summary;
        operation.Description = doc.Description;

       
        if (doc.Parameters != null && operation.Parameters != null)
        {
            foreach (var param in doc.Parameters)
            {
                var openApiParam = operation.Parameters.FirstOrDefault(p => p.Name == param.Key);
                if (openApiParam != null)
                    openApiParam.Description = param.Value;
            }
        }

       
        if (doc.Responses != null)
        {
            foreach (var response in doc.Responses)
            {
                var statusCode = response.Key.ToString();
                if (!operation.Responses.TryGetValue(statusCode, out var openApiResponse)) continue;
                openApiResponse.Description = response.Value;

             
                if (statusCode is not ("200" or "201") || actionName == "UpdateAnswer") continue;
                if (openApiResponse.Content?.TryGetValue("application/json", out var mediaType) == true)
                {
                    mediaType.Example = GetAnswerExample();
                }
            }
        }

      
        AddRequestBodyExample(operation, actionName);

        return Task.CompletedTask;
    }

    private void AddRequestBodyExample(OpenApiOperation operation, string actionName)
    {
        if (actionName is not ("CreateAnswer" or "UpdateAnswer")) return;

        operation.RequestBody ??= new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>()
        };

        var jsonContent = new OpenApiMediaType
        {
            Schema = new OpenApiSchema
            {
                Type = "object",
                Properties =
                {
                    ["length_min"] = new OpenApiSchema { Type = "integer", Format = "int32" },
                    ["length_max"] = new OpenApiSchema { Type = "integer", Format = "int32" },
                    ["first_day_skiing"] = new OpenApiSchema { Type = "boolean" },
                    ["prefer_weekends"] = new OpenApiSchema { Type = "boolean" },
                    ["availability"] = new OpenApiSchema
                    {
                        Type = "object",
                        Description = "Availability dates with user choices (DateChoices enum values)",
                        AdditionalProperties = new OpenApiSchema
                        {
                            Type = "integer",
                            Description = "DateChoices enum value"
                        },
                      
                    }
                }
            }
            ,
            Example = GetAnswerExample()
        };

        operation.RequestBody.Content["application/json"] = jsonContent;
    }

    private OpenApiObject GetAnswerExample()
    {
        return new OpenApiObject
        {
            ["length_min"] = new OpenApiInteger(4),
            ["length_max"] = new OpenApiInteger(8),
            ["first_day_skiing"] = new OpenApiBoolean(true),
            ["prefer_weekends"] = new OpenApiBoolean(false),
            ["availability"] = new OpenApiArray
            {
                new OpenApiObject
                {
                    ["2025-11-20"] = new OpenApiInteger(1), 
                    ["2025-12-21"] = new OpenApiInteger(1)
                },
                new OpenApiObject
                {
                    ["2025-10-20"] = new OpenApiInteger(0), 
                    ["2025-12-21"] = new OpenApiInteger(2)
                }
            }
        };
    }
}