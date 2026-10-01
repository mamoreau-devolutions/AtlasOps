namespace AtlasOps.Geography.Data;

using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;

using AtlasOps.Geography.Domain;

public sealed class GeographyDatasetLoader
{
    private const string SourceName = "countries-data-json";
    private const string SourceUrl = "https://github.com/countries/countries-data-json";

    public async Task<GeographyCatalog> LoadAsync(
        string dataDirectory,
        GeographyOverrideSet overrides,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dataDirectory);
        string countriesDirectory = Path.Combine(dataDirectory, "countries");
        string subdivisionsDirectory = Path.Combine(dataDirectory, "subdivisions");
        EnsureDirectory(countriesDirectory);
        EnsureDirectory(subdivisionsDirectory);

        List<Country> countries = [];
        foreach (string file in Directory.EnumerateFiles(countriesDirectory, "*.json").Order(StringComparer.Ordinal))
        {
            countries.AddRange(await ParseCountriesAsync(file, cancellationToken));
        }

        List<AdministrativeDivision> subdivisions = [];
        foreach (string file in Directory.EnumerateFiles(subdivisionsDirectory, "*.json").Order(StringComparer.Ordinal))
        {
            string countryCode = Path.GetFileNameWithoutExtension(file).ToUpperInvariant();
            subdivisions.AddRange(await ParseSubdivisionsAsync(file, countryCode, cancellationToken));
        }

        Dictionary<string, int> subdivisionCounts = subdivisions
            .GroupBy(static subdivision => subdivision.CountryCode, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(static group => group.Key, static group => group.Count(), StringComparer.OrdinalIgnoreCase);
        countries = countries
            .Select(country => country with
            {
                SubdivisionCount = subdivisionCounts.GetValueOrDefault(country.Alpha2),
            })
            .OrderBy(static country => country.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
        subdivisions.Sort(static (left, right) =>
        {
            int countryComparison = StringComparer.OrdinalIgnoreCase.Compare(left.CountryCode, right.CountryCode);
            return countryComparison != 0
                ? countryComparison
                : StringComparer.OrdinalIgnoreCase.Compare(left.Name, right.Name);
        });

        GeographyCatalog baseCatalog = new(
            countries,
            subdivisions,
            overrides,
            await CreateReleaseAsync(dataDirectory, cancellationToken));
        return GeographyOverrideApplicator.Apply(baseCatalog);
    }

    private static async Task<IReadOnlyList<Country>> ParseCountriesAsync(
        string file,
        CancellationToken cancellationToken)
    {
        await using FileStream stream = File.OpenRead(file);
        using JsonDocument document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        List<Country> countries = [];
        foreach (JsonProperty property in document.RootElement.EnumerateObject())
        {
            JsonElement value = property.Value;
            countries.Add(new Country(
                GetString(value, "alpha2", property.Name),
                GetString(value, "alpha3"),
                GetString(value, "number"),
                GetString(value, "iso_short_name", property.Name),
                GetString(value, "iso_long_name"),
                GetString(value, "continent"),
                GetString(value, "region"),
                GetString(value, "subregion"),
                GetString(value, "currency_code"),
                GetString(value, "country_code"),
                GetString(value, "nationality"),
                GetString(value, "start_of_week"),
                GetString(value, "distance_unit"),
                GetBoolean(value, "un_member"),
                GetBoolean(value, "g20_member"),
                GetBoolean(value, "g7_member"),
                GetCoordinate(value),
                GetStrings(value, "languages_official"),
                GetStrings(value, "languages_spoken"),
                GetStrings(value, "unofficial_names"),
                GetString(value, "postal_code_format"),
                GetFirstString(value, "tld"),
                0));
        }

        return countries;
    }

    private static async Task<IReadOnlyList<AdministrativeDivision>> ParseSubdivisionsAsync(
        string file,
        string countryCode,
        CancellationToken cancellationToken)
    {
        await using FileStream stream = File.OpenRead(file);
        using JsonDocument document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        List<AdministrativeDivision> subdivisions = [];
        foreach (JsonProperty property in document.RootElement.EnumerateObject())
        {
            JsonElement value = property.Value;
            string code = GetString(value, "code", property.Name);
            subdivisions.Add(new AdministrativeDivision(
                $"{countryCode}-{code}",
                countryCode,
                code,
                GetString(value, "name", property.Name),
                GetString(value, "type"),
                GetString(value, "parent_code"),
                GetCoordinate(value),
                GetStrings(value, "unofficial_names")));
        }

        return subdivisions;
    }

    private static async Task<ReferenceDataRelease> CreateReleaseAsync(
        string dataDirectory,
        CancellationToken cancellationToken)
    {
        string commitPath = Path.Combine(dataDirectory, "SOURCE_COMMIT");
        string sourceCommit = File.Exists(commitPath)
            ? (await File.ReadAllTextAsync(commitPath, cancellationToken)).Trim()
            : "unknown";
        using IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        IEnumerable<string> files = Directory
            .EnumerateFiles(dataDirectory, "*.json", SearchOption.AllDirectories)
            .Order(StringComparer.Ordinal);
        foreach (string file in files)
        {
            hash.AppendData(System.Text.Encoding.UTF8.GetBytes(Path.GetRelativePath(dataDirectory, file)));
            await using FileStream stream = File.OpenRead(file);
            byte[] buffer = new byte[81920];
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, cancellationToken)) > 0)
            {
                hash.AppendData(buffer.AsSpan(0, bytesRead));
            }
        }

        return new ReferenceDataRelease(
            SourceName,
            SourceUrl,
            "MIT",
            sourceCommit,
            Convert.ToHexString(hash.GetHashAndReset()).ToLowerInvariant(),
            DateTimeOffset.UtcNow);
    }

    private static void EnsureDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"Geography reference-data directory was not found: {path}");
        }
    }

    private static string GetString(JsonElement element, string propertyName, string fallback = "")
    {
        return element.TryGetProperty(propertyName, out JsonElement property)
            && property.ValueKind == JsonValueKind.String
                ? property.GetString() ?? fallback
                : fallback;
    }

    private static bool GetBoolean(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out JsonElement property)
            && property.ValueKind is JsonValueKind.True or JsonValueKind.False
            && property.GetBoolean();
    }

    private static IReadOnlyList<string> GetStrings(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement property)
            || property.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        return property
            .EnumerateArray()
            .Where(static item => item.ValueKind == JsonValueKind.String)
            .Select(static item => item.GetString())
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .Cast<string>()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string GetFirstString(JsonElement element, string propertyName)
    {
        return GetStrings(element, propertyName).FirstOrDefault() ?? string.Empty;
    }

    private static GeoCoordinate? GetCoordinate(JsonElement element)
    {
        if (!element.TryGetProperty("geo", out JsonElement geo)
            || !TryGetDouble(geo, "latitude", out double latitude)
            || !TryGetDouble(geo, "longitude", out double longitude))
        {
            return null;
        }

        return new GeoCoordinate(latitude, longitude);
    }

    private static bool TryGetDouble(JsonElement element, string propertyName, out double value)
    {
        value = default;
        if (!element.TryGetProperty(propertyName, out JsonElement property))
        {
            return false;
        }

        if (property.ValueKind == JsonValueKind.Number)
        {
            return property.TryGetDouble(out value);
        }

        return property.ValueKind == JsonValueKind.String
            && double.TryParse(property.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }
}