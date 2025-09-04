namespace Skii.Docs;

public class OperationDocumentation
{
    public OperationDocumentation()
    {
    }

    public OperationDocumentation(string summary, string description, Dictionary<string, string>? parameters, Dictionary<int, string>? responses)
    {
        Summary = summary;
        Description = description;
        Parameters = parameters;
        Responses = responses;
    }

    public string Summary { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, string>? Parameters { get; set; }
    public Dictionary<int, string>? Responses { get; set; }
}