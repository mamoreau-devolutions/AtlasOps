namespace AtlasOps.Connectors.Collaboration;

public sealed record AiProviderDescriptor(
    string Id,
    string DisplayName,
    string PackageId,
    Uri DefaultEndpoint,
    int MaximumPromptCharacters,
    IReadOnlyList<string> Capabilities);

public sealed record AiPromptPlan(
    string ProviderId,
    string Model,
    string SystemInstruction,
    string Prompt,
    double Temperature,
    int MaximumOutputTokens,
    IReadOnlyDictionary<string, string> Metadata);

public sealed record AiPromptValidation(bool Valid, IReadOnlyList<string> Diagnostics);

public static class AiConnectorCatalog
{
    public static IReadOnlyList<AiProviderDescriptor> Providers { get; } =
    [
        new(
            "google-genai",
            "Google Generative AI",
            "Google.GenAI",
            new Uri("https://generativelanguage.googleapis.com/"),
            1_000_000,
            ["chat", "embeddings", "structured output"]),
        new(
            "model-context-protocol",
            "Model Context Protocol",
            "ModelContextProtocol",
            new Uri("https://localhost/"),
            250_000,
            ["tools", "resources", "prompts"]),
        new(
            "kiota-bundle",
            "Kiota client runtime",
            "Microsoft.Kiota.Bundle",
            new Uri("https://graph.microsoft.com/"),
            100_000,
            ["authentication", "HTTP transport", "JSON", "multipart", "form serialization"]),
    ];

    public static AiPromptValidation Validate(AiPromptPlan plan)
    {
        List<string> diagnostics = new();
        AiProviderDescriptor? provider = Providers.FirstOrDefault(
            item => string.Equals(item.Id, plan.ProviderId, StringComparison.OrdinalIgnoreCase));

        if (provider is null)
        {
            diagnostics.Add("The selected AI provider is not registered.");
        }

        if (string.IsNullOrWhiteSpace(plan.Model) || plan.Model.Length > 200)
        {
            diagnostics.Add("Model must contain between 1 and 200 characters.");
        }

        if (string.IsNullOrWhiteSpace(plan.Prompt))
        {
            diagnostics.Add("Prompt cannot be empty.");
        }
        else if (provider is not null && plan.Prompt.Length > provider.MaximumPromptCharacters)
        {
            diagnostics.Add("Prompt exceeds the provider limit.");
        }

        if (plan.Temperature is < 0d or > 2d)
        {
            diagnostics.Add("Temperature must be between zero and two.");
        }

        if (plan.MaximumOutputTokens is < 1 or > 1_000_000)
        {
            diagnostics.Add("Maximum output tokens must be between 1 and 1,000,000.");
        }

        return new AiPromptValidation(diagnostics.Count == 0, diagnostics);
    }
}
