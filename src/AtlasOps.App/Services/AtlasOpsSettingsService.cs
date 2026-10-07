namespace AtlasOps.App.Services;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using AtlasOps.Core;

public interface IAtlasOpsSettingsService
{
    Task<AtlasOpsSettings> LoadSettingsAsync(CancellationToken cancellationToken = default);

    Task SaveSettingsAsync(AtlasOpsSettings settings, CancellationToken cancellationToken = default);

    Task<AtlasOpsLayoutState> LoadLayoutAsync(CancellationToken cancellationToken = default);

    Task SaveLayoutAsync(AtlasOpsLayoutState layout, CancellationToken cancellationToken = default);
}

public sealed class AtlasOpsSettingsService : IAtlasOpsSettingsService
{
    private readonly string settingsPath;
    private readonly string layoutPath;

    public AtlasOpsSettingsService(string dataPath)
    {
        if (string.IsNullOrWhiteSpace(dataPath))
        {
            throw new ArgumentException("A settings data path is required.", nameof(dataPath));
        }

        Directory.CreateDirectory(dataPath);
        this.settingsPath = Path.Combine(dataPath, "settings.json");
        this.layoutPath = Path.Combine(dataPath, "layout.json");
    }

    public async Task<AtlasOpsSettings> LoadSettingsAsync(CancellationToken cancellationToken = default)
    {
        return await LoadOrDefaultAsync(
            this.settingsPath,
            new AtlasOpsSettings(),
            AtlasOpsSettingsJsonContext.Default.AtlasOpsSettings,
            cancellationToken);
    }

    public Task SaveSettingsAsync(AtlasOpsSettings settings, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        return SaveAsync(this.settingsPath, settings, AtlasOpsSettingsJsonContext.Default.AtlasOpsSettings, cancellationToken);
    }

    public async Task<AtlasOpsLayoutState> LoadLayoutAsync(CancellationToken cancellationToken = default)
    {
        return await LoadOrDefaultAsync(
            this.layoutPath,
            new AtlasOpsLayoutState(),
            AtlasOpsSettingsJsonContext.Default.AtlasOpsLayoutState,
            cancellationToken);
    }

    public Task SaveLayoutAsync(AtlasOpsLayoutState layout, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(layout);
        return SaveAsync(this.layoutPath, layout, AtlasOpsSettingsJsonContext.Default.AtlasOpsLayoutState, cancellationToken);
    }

    private static async Task<T> LoadOrDefaultAsync<T>(
        string path,
        T defaultValue,
        JsonTypeInfo<T> typeInfo,
        CancellationToken cancellationToken)
    {
        if (!File.Exists(path))
        {
            return defaultValue;
        }

        await using FileStream stream = File.OpenRead(path);
        T? value = await JsonSerializer.DeserializeAsync(stream, typeInfo, cancellationToken);
        return value ?? defaultValue;
    }

    private static async Task SaveAsync<T>(
        string path,
        T value,
        JsonTypeInfo<T> typeInfo,
        CancellationToken cancellationToken)
    {
        string temporaryPath = $"{path}.{Guid.NewGuid():N}.tmp";
        try
        {
            await using (FileStream stream = File.Create(temporaryPath))
            {
                await JsonSerializer.SerializeAsync(stream, value, typeInfo, cancellationToken);
            }

            File.Move(temporaryPath, path, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }
}

[JsonSourceGenerationOptions(
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(AtlasOpsSettings))]
[JsonSerializable(typeof(AtlasOpsLayoutState))]
internal sealed partial class AtlasOpsSettingsJsonContext : JsonSerializerContext
{
}
