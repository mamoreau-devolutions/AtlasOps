namespace AtlasOps.ReferenceData.Infrastructure;

using System.Text.Json;
using System.Text.Json.Serialization;

public sealed record ReferenceDataRow(
    string Id,
    string Category,
    string Name,
    string Details,
    string SearchText);

public sealed record ReferenceValidationIssue(
    string Severity,
    string Code,
    string Message,
    string RecordId);

public sealed record ReferencePackManifest(
    string Name,
    string Version,
    string License,
    DateTimeOffset RetrievedAt,
    IReadOnlyList<string> Sources,
    int FileCount,
    IReadOnlyList<ReferenceManifestFile> Files);

public sealed record ReferenceManifestFile(
    string Path,
    long Bytes,
    string Sha256);

public sealed record ReferenceWorkbenchSnapshot(
    string Title,
    string Description,
    ReferencePackManifest Manifest,
    IReadOnlyList<ReferenceDataRow> Rows,
    IReadOnlyList<ReferenceValidationIssue> Issues,
    IReadOnlyDictionary<string, string> Metrics);

public static class ReferenceDataPaths
{
    public static string Root => Path.Combine(
        AppContext.BaseDirectory,
        "ReferenceData",
        "ReferenceIntelligence");

    public static string GetPackDirectory(string name)
    {
        return Path.Combine(Root, name);
    }
}

public static class ReferenceManifestLoader
{
    public static async Task<ReferencePackManifest> LoadAsync(
        string directory,
        CancellationToken cancellationToken = default)
    {
        string path = Path.Combine(directory, "manifest.json");
        await using FileStream stream = File.OpenRead(path);
        ReferencePackManifest? manifest =
            await JsonSerializer.DeserializeAsync(
                stream,
                ReferenceDataJsonContext.Default.ReferencePackManifest,
                cancellationToken);
        return manifest ?? throw new InvalidDataException($"Manifest '{path}' is empty.");
    }
}

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(ReferencePackManifest))]
internal sealed partial class ReferenceDataJsonContext : JsonSerializerContext
{
}
