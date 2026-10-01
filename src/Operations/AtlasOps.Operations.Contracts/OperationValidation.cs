namespace AtlasOps.Operations.Contracts;

public sealed record OperationValidationResult(
    bool Valid,
    IReadOnlyList<string> Diagnostics);

public static class OperationValidator
{
    public static OperationValidationResult Validate(OperationDescriptor descriptor)
    {
        List<string> diagnostics = [];

        AddRequired(diagnostics, descriptor.Id, "Operation ID");
        AddRequired(diagnostics, descriptor.DisplayName, "Display name");
        AddRequired(diagnostics, descriptor.ProviderFamily, "Provider family");

        if (descriptor.MaximumConcurrency is < 1 or > 1_024)
        {
            diagnostics.Add("Maximum concurrency must be between 1 and 1,024.");
        }

        if (descriptor.MaximumAttempts is < 1 or > 100)
        {
            diagnostics.Add("Maximum attempts must be between 1 and 100.");
        }

        if (descriptor.Timeout < TimeSpan.FromMilliseconds(100) ||
            descriptor.Timeout > TimeSpan.FromHours(24))
        {
            diagnostics.Add("Timeout must be between 100 milliseconds and 24 hours.");
        }

        return new OperationValidationResult(diagnostics.Count == 0, diagnostics);
    }

    public static OperationValidationResult Validate(OperationEnvelope envelope)
    {
        List<string> diagnostics = [];

        if (envelope.Id == Guid.Empty)
        {
            diagnostics.Add("Operation ID cannot be empty.");
        }

        AddRequired(diagnostics, envelope.ProviderId, "Provider ID");
        AddRequired(diagnostics, envelope.Operation, "Operation");

        if (envelope.CreatedAt == default)
        {
            diagnostics.Add("Created timestamp is required.");
        }

        foreach (KeyValuePair<string, string> parameter in envelope.Parameters)
        {
            AddRequired(diagnostics, parameter.Key, "Parameter key");
            if (parameter.Value.Length > 1_000_000)
            {
                diagnostics.Add($"Parameter '{parameter.Key}' exceeds the one-million-character limit.");
            }
        }

        return new OperationValidationResult(diagnostics.Count == 0, diagnostics);
    }

    private static void AddRequired(List<string> diagnostics, string? value, string displayName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            diagnostics.Add($"{displayName} is required.");
        }
    }
}
