namespace PromptOptimizer.Infrastructure.OpenAI;

/// <summary>
/// Strongly-typed configuration for the LLM provider. Supports both OpenAI and
/// Azure OpenAI. Bound from the "OpenAI" configuration section.
/// </summary>
public class OpenAIOptions
{
    public const string SectionName = "OpenAI";

    /// <summary>"OpenAI" or "AzureOpenAI".</summary>
    public string Provider { get; set; } = "OpenAI";

    /// <summary>API key for OpenAI or Azure OpenAI. Leave empty to run in offline/mock mode.</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Model (OpenAI) or deployment name (Azure OpenAI).</summary>
    public string Model { get; set; } = "gpt-4o-mini";

    /// <summary>Azure OpenAI resource endpoint, e.g. https://my-resource.openai.azure.com.</summary>
    public string? Endpoint { get; set; }

    /// <summary>Azure OpenAI REST API version.</summary>
    public string ApiVersion { get; set; } = "2024-08-01-preview";

    public double Temperature { get; set; } = 0.3;

    public int MaxTokens { get; set; } = 1500;

    public int TimeoutSeconds { get; set; } = 60;

    public bool IsAzure =>
        string.Equals(Provider, "AzureOpenAI", StringComparison.OrdinalIgnoreCase);

    public bool IsConfigured => !string.IsNullOrWhiteSpace(ApiKey);
}
