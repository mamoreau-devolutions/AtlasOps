namespace AtlasOps.App.Services;

using System.Text.Json;
using System.Text.Json.Serialization;

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
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

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
        return await LoadOrDefaultAsync(this.settingsPath, new AtlasOpsSettings(), cancellationToken);
    }

    public Task SaveSettingsAsync(AtlasOpsSettings settings, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        return SaveAsync(this.settingsPath, settings, cancellationToken);
    }

    public async Task<AtlasOpsLayoutState> LoadLayoutAsync(CancellationToken cancellationToken = default)
    {
        return await LoadOrDefaultAsync(this.layoutPath, new AtlasOpsLayoutState(), cancellationToken);
    }

    public Task SaveLayoutAsync(AtlasOpsLayoutState layout, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(layout);
        return SaveAsync(this.layoutPath, layout, cancellationToken);
    }

    private static async Task<T> LoadOrDefaultAsync<T>(
        string path,
        T defaultValue,
        CancellationToken cancellationToken)
    {
        if (!File.Exists(path))
        {
            return defaultValue;
        }

        await using FileStream stream = File.OpenRead(path);
        T? value = await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions, cancellationToken);
        return value ?? defaultValue;
    }

    private static async Task SaveAsync<T>(string path, T value, CancellationToken cancellationToken)
    {
        string temporaryPath = $"{path}.{Guid.NewGuid():N}.tmp";
        try
        {
            await using (FileStream stream = File.Create(temporaryPath))
            {
                await JsonSerializer.SerializeAsync(stream, value, JsonOptions, cancellationToken);
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