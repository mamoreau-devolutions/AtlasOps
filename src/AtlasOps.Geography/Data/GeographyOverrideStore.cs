namespace AtlasOps.Geography.Data;

using System.Text.Json;

using AtlasOps.Geography.Domain;

public sealed class GeographyOverrideStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    private readonly string filePath;

    public GeographyOverrideStore(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        this.filePath = filePath;
    }

    public async Task<GeographyOverrideSet> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(this.filePath))
        {
            return GeographyOverrideSet.Empty;
        }

        await using FileStream stream = File.OpenRead(this.filePath);
        GeographyOverrideSet? value = await JsonSerializer.DeserializeAsync<GeographyOverrideSet>(
            stream,
            SerializerOptions,
            cancellationToken);
        if (value is null || value.SchemaVersion != 1)
        {
            throw new InvalidDataException("The geography override file uses an unsupported schema.");
        }

        return value;
    }

    public async Task SaveAsync(
        GeographyOverrideSet overrides,
        CancellationToken cancellationToken = default)
    {
        string directory = Path.GetDirectoryName(this.filePath)
            ?? throw new InvalidOperationException("The geography override path has no parent directory.");
        Directory.CreateDirectory(directory);
        string temporaryPath = $"{this.filePath}.{Guid.NewGuid():N}.tmp";
        try
        {
            await using (FileStream stream = File.Create(temporaryPath))
            {
                await JsonSerializer.SerializeAsync(stream, overrides, SerializerOptions, cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }

            File.Move(temporaryPath, this.filePath, true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    public async Task ExportAsync(string destinationPath, CancellationToken cancellationToken = default)
    {
        GeographyOverrideSet overrides = await this.LoadAsync(cancellationToken);
        GeographyOverrideStore destination = new(destinationPath);
        await destination.SaveAsync(overrides, cancellationToken);
    }

    public async Task<GeographyOverrideSet> ImportAsync(
        string sourcePath,
        CancellationToken cancellationToken = default)
    {
        GeographyOverrideStore source = new(sourcePath);
        GeographyOverrideSet overrides = await source.LoadAsync(cancellationToken);
        await this.SaveAsync(overrides, cancellationToken);
        return overrides;
    }
}