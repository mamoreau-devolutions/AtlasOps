namespace AtlasOps.Product.Localization;

public sealed record LocalizedValue(
    string Key,
    string Culture,
    string Value,
    string SourceCulture);

public sealed class LocalizationCatalog
{
    private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> values;
    private readonly string defaultCulture;

    public LocalizationCatalog(
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> values,
        string defaultCulture)
    {
        this.values = values;
        this.defaultCulture = defaultCulture;
    }

    public LocalizedValue Resolve(string key, string culture)
    {
        foreach (string candidate in BuildFallbackChain(culture))
        {
            if (this.values.TryGetValue(candidate, out IReadOnlyDictionary<string, string>? catalog) &&
                catalog.TryGetValue(key, out string? value))
            {
                return new LocalizedValue(key, culture, value, candidate);
            }
        }

        return new LocalizedValue(key, culture, $"[{key}]", string.Empty);
    }

    public IReadOnlyList<string> Validate()
    {
        if (!this.values.TryGetValue(this.defaultCulture, out IReadOnlyDictionary<string, string>? baseline))
        {
            return [$"Default culture '{this.defaultCulture}' is missing."];
        }

        List<string> diagnostics = [];
        foreach (KeyValuePair<string, IReadOnlyDictionary<string, string>> culture in this.values)
        {
            foreach (string key in baseline.Keys)
            {
                if (!culture.Value.ContainsKey(key))
                {
                    diagnostics.Add($"Culture '{culture.Key}' is missing key '{key}'.");
                }
            }
        }

        return diagnostics;
    }

    private IEnumerable<string> BuildFallbackChain(string culture)
    {
        string current = culture;
        HashSet<string> emitted = new(StringComparer.OrdinalIgnoreCase);
        while (!string.IsNullOrWhiteSpace(current))
        {
            if (emitted.Add(current))
            {
                yield return current;
            }

            int separator = current.LastIndexOf('-');
            current = separator > 0 ? current[..separator] : string.Empty;
        }

        if (emitted.Add(this.defaultCulture))
        {
            yield return this.defaultCulture;
        }
    }
}
