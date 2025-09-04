using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Skii.Docs;

public class AuthControllerOperationTransformer : IOpenApiOperationTransformer
{
    private readonly Dictionary<string, OperationDocumentation> _documentation = new()
    {
        ["GoogleSignin"] = new OperationDocumentation
        {
            Summary = "Initiate Google Sign-In",
            Description = "Redirects the user to Google's OAuth 2.0 login page. After authentication, Google will redirect to the GoogleCallback endpoint.",
            Parameters = null,
            Responses = new Dictionary<int, string>
            {
                [302] = "Redirects to Google login page."
            }
        },
        ["GoogleCallback"] = new OperationDocumentation
        {
            Summary = "Handle Google OAuth callback",
            Description = "Receives authentication data from Google. Creates a new user if needed and returns a JWT token with user information.",
            Parameters = null,
            Responses = new Dictionary<int, string>
            {
                [200] = "Authentication successful. Returns JWT token, email, and name.",
                [400] = "Google authentication failed.",
                [500] = "Server error. Missing Google Id."
            }
        },
        ["GetAllUsers"] = new OperationDocumentation
        {
            Summary = "Retrieve all users",
            Description = "Returns a list of all registered users.",
            Parameters = null,
            Responses = new Dictionary<int, string>
            {
                [200] = "Returns a list of users.",
                [404] = "No users found."
            }
        }
    };

    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var actionName = context.Description.ActionDescriptor.RouteValues["action"];
        if (actionName == null || !_documentation.TryGetValue(actionName, out var doc)) return Task.CompletedTask;
        operation.Summary = doc.Summary;
        operation.Description = doc.Description;

        if (doc.Responses != null)
        {
            foreach (var response in doc.Responses)
            {
                var statusCode = response.Key.ToString();
                if (operation.Responses.TryGetValue(statusCode, out var openApiResponse))
                    openApiResponse.Description = response.Value;
            }
        }

        AddExamples(operation, actionName);

        return Task.CompletedTask;
    }

    private void AddExamples(OpenApiOperation operation, string actionName)
    {
        switch (actionName)
        {
            case "GoogleCallback":
            {
                if (operation.Responses.TryGetValue("200", out OpenApiResponse? value) &&
                    value.Content?.TryGetValue("application/json", out var value1) is true)
                {
                        value1.Example = new OpenApiObject
                    {
                        ["token"] = new OpenApiString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."),
                        ["email"] = new OpenApiString("john@example.com"),
                        ["name"] = new OpenApiString("John Doe")
                    };
                }

                break;
            }
            case "GetAllUsers":
            {
                if (operation.Responses.TryGetValue("200", out OpenApiResponse? value) &&
                    value.Content?.TryGetValue("application/json", out var value1) is true)
                {
                        value1.Example = new OpenApiArray
                    {
                        new OpenApiObject
                        {
                            ["id"] = new OpenApiString("123e4567-e89b-12d3-a456-426614174000"),
                            ["name"] = new OpenApiString("John Doe"),
                            ["email"] = new OpenApiString("john@example.com"),
                            ["googleId"] = new OpenApiString("google-12345")
                        }
                    };
                }

                break;
            }
        }
    }
}
