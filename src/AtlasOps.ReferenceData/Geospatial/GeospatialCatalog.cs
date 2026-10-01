namespace AtlasOps.ReferenceData.Geospatial;

using System.Globalization;
using System.Text.Json;

using AtlasOps.ReferenceData.Infrastructure;

public sealed record GeoBounds(
    double West,
    double South,
    double East,
    double North)
{
    public double CenterLongitude => (this.West + this.East) / 2;

    public double CenterLatitude => (this.South + this.North) / 2;
}

public sealed record GeoFeatureSummary(
    string Id,
    string Layer,
    string Name,
    string FeatureClass,
    string IsoCode,
    string GeometryType,
    GeoBounds Bounds);

public sealed record GeospatialCatalog(
    ReferencePackManifest Manifest,
    IReadOnlyList<GeoFeatureSummary> Features);

public sealed class GeospatialCatalogLoader
{
    public async Task<GeospatialCatalog> LoadAsync(
        string directory,
        CancellationToken cancellationToken = default)
    {
        ReferencePackManifest manifest = await ReferenceManifestLoader.LoadAsync(directory, cancellationToken);
        List<GeoFeatureSummary> features = [];
        foreach (string path in Directory.EnumerateFiles(directory, "*.geojson").Order(StringComparer.Ordinal))
        {
            await using FileStream stream = File.OpenRead(path);
            using JsonDocument document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            string layer = Path.GetFileNameWithoutExtension(path);
            int index = 0;
            foreach (JsonElement feature in document.RootElement.GetProperty("features").EnumerateArray())
            {
                JsonElement properties = feature.GetProperty("properties");
                JsonElement geometry = feature.GetProperty("geometry");
                GeoBounds bounds = CalculateBounds(geometry.GetProperty("coordinates"));
                string name = ReadProperty(properties, "name", "NAME", "name_en", "NAME_EN");
                string featureClass = ReadProperty(properties, "featurecla", "FEATURECLA", "type", "TYPE");
                string isoCode = ReadProperty(properties, "iso_a2", "ISO_A2", "adm0_a3", "ADM0_A3", "iso_3166_2");
                features.Add(new GeoFeatureSummary(
                    $"{layer}:{index++}",
                    layer,
                    string.IsNullOrWhiteSpace(name) ? "Unnamed feature" : name,
                    featureClass,
                    isoCode,
                    geometry.GetProperty("type").GetString() ?? "Unknown",
                    bounds));
            }
        }

        return new GeospatialCatalog(manifest, features);
    }

    private static string ReadProperty(JsonElement properties, params string[] names)
    {
        foreach (string name in names)
        {
            if (properties.TryGetProperty(name, out JsonElement value))
            {
                return value.ValueKind switch
                {
                    JsonValueKind.String => value.GetString() ?? string.Empty,
                    JsonValueKind.Number => value.GetRawText(),
                    _ => string.Empty,
                };
            }
        }

        return string.Empty;
    }

    private static GeoBounds CalculateBounds(JsonElement coordinates)
    {
        BoundsAccumulator accumulator = new();
        Accumulate(coordinates, accumulator);
        return accumulator.HasValue
            ? new GeoBounds(accumulator.West, accumulator.South, accumulator.East, accumulator.North)
            : new GeoBounds(0, 0, 0, 0);
    }

    private static void Accumulate(JsonElement element, BoundsAccumulator accumulator)
    {
        if (element.ValueKind != JsonValueKind.Array)
        {
            return;
        }

        JsonElement.ArrayEnumerator values = element.EnumerateArray();
        if (values.MoveNext())
        {
            JsonElement first = values.Current;
            if (first.ValueKind == JsonValueKind.Number &&
                values.MoveNext() &&
                values.Current.ValueKind == JsonValueKind.Number)
            {
                accumulator.Add(first.GetDouble(), values.Current.GetDouble());
                return;
            }
        }

        foreach (JsonElement child in element.EnumerateArray())
        {
            Accumulate(child, accumulator);
        }
    }

    private sealed class BoundsAccumulator
    {
        public bool HasValue { get; private set; }

        public double West { get; private set; } = double.MaxValue;

        public double South { get; private set; } = double.MaxValue;

        public double East { get; private set; } = double.MinValue;

        public double North { get; private set; } = double.MinValue;

        public void Add(double longitude, double latitude)
        {
            this.HasValue = true;
            this.West = Math.Min(this.West, longitude);
            this.South = Math.Min(this.South, latitude);
            this.East = Math.Max(this.East, longitude);
            this.North = Math.Max(this.North, latitude);
        }
    }
}

public sealed class GeospatialAnalysisService
{
    private const double EarthRadiusKilometers = 6371.0088;

    public double DistanceKilometers(
        double latitude1,
        double longitude1,
        double latitude2,
        double longitude2)
    {
        double latitudeDelta = DegreesToRadians(latitude2 - latitude1);
        double longitudeDelta = DegreesToRadians(longitude2 - longitude1);
        double firstLatitude = DegreesToRadians(latitude1);
        double secondLatitude = DegreesToRadians(latitude2);
        double haversine =
            Math.Pow(Math.Sin(latitudeDelta / 2), 2) +
            Math.Cos(firstLatitude) *
            Math.Cos(secondLatitude) *
            Math.Pow(Math.Sin(longitudeDelta / 2), 2);
        return EarthRadiusKilometers * 2 * Math.Asin(Math.Sqrt(haversine));
    }

    public IReadOnlyList<GeoFeatureSummary> FindNearest(
        GeospatialCatalog catalog,
        double latitude,
        double longitude,
        int limit = 25)
    {
        return catalog.Features
            .OrderBy(item => this.DistanceKilometers(
                latitude,
                longitude,
                item.Bounds.CenterLatitude,
                item.Bounds.CenterLongitude))
            .Take(limit)
            .ToArray();
    }

    public IReadOnlyList<GeoFeatureSummary> Intersect(
        GeospatialCatalog catalog,
        GeoBounds query)
    {
        return catalog.Features
            .Where(item =>
                LatitudeIntersects(item.Bounds, query) &&
                LongitudeIntersects(item.Bounds, query))
            .ToArray();
    }

    public IReadOnlyList<GeoFeatureSummary> FindByCountryCode(
        GeospatialCatalog catalog,
        string isoCode)
    {
        return catalog.Features
            .Where(item => item.IsoCode.Equals(isoCode, StringComparison.OrdinalIgnoreCase))
            .OrderBy(static item => item.Layer, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static item => item.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static bool LatitudeIntersects(GeoBounds item, GeoBounds query)
    {
        return item.North >= query.South && item.South <= query.North;
    }

    private static bool LongitudeIntersects(GeoBounds item, GeoBounds query)
    {
        if (query.West <= query.East)
        {
            return item.East >= query.West && item.West <= query.East;
        }

        return item.East >= query.West || item.West <= query.East;
    }

    private static double DegreesToRadians(double value)
    {
        return value * Math.PI / 180;
    }
}

public static class GeospatialWorkbenchBuilder
{
    public static async Task<ReferenceWorkbenchSnapshot> BuildAsync(CancellationToken cancellationToken = default)
    {
        GeospatialCatalog catalog = await new GeospatialCatalogLoader().LoadAsync(
            ReferenceDataPaths.GetPackDirectory("geospatial"),
            cancellationToken);
        List<ReferenceValidationIssue> issues = catalog.Features
            .Where(static item =>
                item.Bounds.West < -180 ||
                item.Bounds.East > 180 ||
                item.Bounds.South < -90 ||
                item.Bounds.North > 90)
            .Select(static item => new ReferenceValidationIssue(
                "Error",
                "GEO-COORDINATE-RANGE",
                "Feature bounds exceed WGS84 coordinate limits.",
                item.Id))
            .ToList();
        IReadOnlyList<ReferenceDataRow> rows = catalog.Features
            .Select(static item => new ReferenceDataRow(
                item.Id,
                HumanizeLayer(item.Layer),
                item.Name,
                $"{item.FeatureClass} · {item.IsoCode} · {item.GeometryType} · {item.Bounds.West:N3},{item.Bounds.South:N3} to {item.Bounds.East:N3},{item.Bounds.North:N3}",
                $"{item.Name} {item.FeatureClass} {item.IsoCode} {item.Layer} {item.GeometryType}"))
            .ToArray();
        return new ReferenceWorkbenchSnapshot(
            "Natural Earth geospatial operations",
            "Searchable global features with validated bounds, nearest-feature queries, and spatial intersection.",
            catalog.Manifest,
            rows,
            issues,
            new Dictionary<string, string>
            {
                ["Features"] = catalog.Features.Count.ToString("N0", CultureInfo.InvariantCulture),
                ["Layers"] = catalog.Features.Select(static item => item.Layer).Distinct().Count().ToString("N0", CultureInfo.InvariantCulture),
                ["Named places"] = catalog.Features.Count(static item => item.Name != "Unnamed feature").ToString("N0", CultureInfo.InvariantCulture),
                ["ISO-linked features"] = catalog.Features.Count(static item => !string.IsNullOrWhiteSpace(item.IsoCode)).ToString("N0", CultureInfo.InvariantCulture),
            });
    }

    private static string HumanizeLayer(string layer)
    {
        return layer
            .Replace("ne_110m_", string.Empty, StringComparison.Ordinal)
            .Replace("ne_50m_", string.Empty, StringComparison.Ordinal)
            .Replace("ne_10m_", string.Empty, StringComparison.Ordinal)
            .Replace('_', ' ');
    }
}