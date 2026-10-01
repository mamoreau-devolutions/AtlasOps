namespace AtlasOps.Tests;

using AtlasOps.ReferenceData.Cloud;
using AtlasOps.ReferenceData.Geospatial;
using AtlasOps.ReferenceData.Iana;
using AtlasOps.ReferenceData.Infrastructure;
using AtlasOps.ReferenceData.Lifecycle;
using AtlasOps.ReferenceData.Localization;

[TestClass]
public sealed class ReferenceIntelligenceTests
{
    [TestMethod]
    public async Task IanaLoaderBuildsPortProtocolAndCipherCatalog()
    {
        IanaCatalog catalog = await new IanaCatalogLoader().LoadAsync(
            ReferenceDataPaths.GetPackDirectory("iana"));
        IanaRegistryAnalyzer analyzer = new();

        IReadOnlyList<IanaServiceAssignment> https = analyzer.FindAssignments(catalog, 443, "tcp");

        Assert.IsGreaterThan(10_000, catalog.Services.Count);
        Assert.IsGreaterThan(100, catalog.Protocols.Count);
        Assert.IsGreaterThan(300, catalog.CipherSuites.Count);
        Assert.IsNotEmpty(https);
        Assert.IsTrue(https.Any(static item =>
            item.ServiceName.Equals("https", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public async Task LifecycleLoaderBuildsUpgradeCampaign()
    {
        LifecycleCatalog catalog = await new LifecycleCatalogLoader().LoadAsync(
            ReferenceDataPaths.GetPackDirectory("lifecycle"));
        LifecycleRiskAnalyzer analyzer = new();

        IReadOnlyList<LifecycleRelease> campaign = analyzer.CreateUpgradeCampaign(
            catalog,
            DateOnly.FromDateTime(DateTime.UtcNow),
            365);

        Assert.IsGreaterThan(1_000, catalog.Releases.Count);
        Assert.IsGreaterThan(10_000, catalog.VersionCount);
        Assert.IsNotEmpty(campaign);
        Assert.IsTrue(catalog.Releases.Any(static item => item.Product == "akeneo-pim"));
    }

    [TestMethod]
    public async Task LocalizationLoaderBuildsFallbackAndZoneMappings()
    {
        LocalizationCatalog catalog = await new LocalizationCatalogLoader().LoadAsync(
            ReferenceDataPaths.GetPackDirectory("localization"));
        IReadOnlySet<string> locales = catalog.Locales
            .Select(static item => item.Locale)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        IReadOnlyList<string> fallback = new LocaleFallbackResolver().Resolve("zh-Hant-TW", locales);

        Assert.IsGreaterThan(500, catalog.Locales.Count);
        Assert.IsGreaterThan(300, catalog.TimeZones.Count);
        Assert.IsGreaterThan(400, catalog.WindowsMappings.Count);
        Assert.IsNotEmpty(fallback);
        Assert.AreEqual("zh-Hant", fallback[0]);
    }

    [TestMethod]
    public async Task CloudLoaderRanksCrossProviderOffers()
    {
        CloudCatalog catalog = await new CloudCatalogLoader().LoadAsync(
            ReferenceDataPaths.GetPackDirectory("cloud"));
        CloudCostAnalyzer analyzer = new();

        IReadOnlyList<CloudOffer> ranked = analyzer.Rank(catalog, 4, 16, null, 100);

        Assert.IsGreaterThan(90_000, catalog.Offers.Count);
        Assert.HasCount(100, ranked);
        Assert.IsTrue(ranked.SequenceEqual(ranked.OrderBy(static item => item.HourlyUsd)));
        Assert.AreEqual(3, catalog.Offers.Select(static item => item.Provider).Distinct().Count());
    }

    [TestMethod]
    public async Task GeospatialLoaderSupportsNearestAndAntimeridianQueries()
    {
        GeospatialCatalog catalog = await new GeospatialCatalogLoader().LoadAsync(
            ReferenceDataPaths.GetPackDirectory("geospatial"));
        GeospatialAnalysisService service = new();

        IReadOnlyList<GeoFeatureSummary> nearest = service.FindNearest(
            catalog,
            45.5019,
            -73.5674,
            5);
        IReadOnlyList<GeoFeatureSummary> antimeridian = service.Intersect(
            catalog,
            new GeoBounds(170, -20, -170, 20));

        Assert.IsGreaterThan(5_000, catalog.Features.Count);
        Assert.HasCount(5, nearest);
        Assert.IsNotEmpty(antimeridian);
        Assert.IsInRange(500, 600, service.DistanceKilometers(45.5019, -73.5674, 40.7128, -74.0060));
    }

    [TestMethod]
    public void SearchIndexRanksExactTokensBeforePartialMatches()
    {
        ReferenceDataRow exact = new("1", "Test", "HTTPS", "Port 443", "https port 443");
        ReferenceDataRow partial = new("2", "Test", "HTTPS proxy", "Proxy", "httpsproxy port 8443");
        ReferenceSearchIndex index = new([partial, exact]);

        IReadOnlyList<ReferenceDataRow> results = index.Search("https");

        Assert.HasCount(2, results);
        Assert.AreEqual(exact, results[0]);
    }

    [TestMethod]
    public async Task DirectoryFingerprintIsStableAndChangesWithContent()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"atlasops-reference-integrity-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            await File.WriteAllTextAsync(Path.Combine(directory, "a.txt"), "alpha");
            await File.WriteAllTextAsync(Path.Combine(directory, "b.txt"), "bravo");

            string first = await ReferenceDataIntegrity.ComputeDirectoryFingerprintAsync(directory);
            string second = await ReferenceDataIntegrity.ComputeDirectoryFingerprintAsync(directory);
            await File.WriteAllTextAsync(Path.Combine(directory, "b.txt"), "changed");
            string changed = await ReferenceDataIntegrity.ComputeDirectoryFingerprintAsync(directory);

            Assert.AreEqual(first, second);
            Assert.AreNotEqual(first, changed);
            Assert.HasCount(64, first);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public async Task QuotedCsvReaderParsesCommasEscapedQuotesAndMultilineFields()
    {
        string path = Path.Combine(Path.GetTempPath(), $"atlasops-reference-csv-{Guid.NewGuid():N}.csv");
        try
        {
            await File.WriteAllTextAsync(
                path,
                "name,description,empty\r\n\"service, one\",\"Uses \"\"quoted\"\"\r\ntext\",\r\n");

            List<IReadOnlyList<string>> rows = [];
            await foreach (IReadOnlyList<string> row in QuotedCsvReader.ReadAsync(path))
            {
                rows.Add(row);
            }

            Assert.HasCount(2, rows);
            Assert.AreEqual("service, one", rows[1][0]);
            Assert.AreEqual("Uses \"quoted\"\r\ntext", rows[1][1]);
            Assert.AreEqual(string.Empty, rows[1][2]);
        }
        finally
        {
            File.Delete(path);
        }
    }
}