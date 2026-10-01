namespace AtlasOps.Tests;

using AtlasOps.Geography.Data;
using AtlasOps.Geography.Domain;
using AtlasOps.Geography.Services;

[TestClass]
public sealed class GeographyCatalogTests
{
    private static string DataDirectory =>
        Path.Combine(AppContext.BaseDirectory, "ReferenceData", "Geography");

    [TestMethod]
    public async Task LoaderBuildsCompleteValidatedCatalog()
    {
        GeographyDatasetLoader loader = new();
        GeographyCatalog catalog = await loader.LoadAsync(
            DataDirectory,
            GeographyOverrideSet.Empty);
        GeographyValidationReport report = new GeographyCatalogValidator().Validate(catalog);

        Assert.IsGreaterThanOrEqualTo(249, catalog.Countries.Count);
        Assert.IsGreaterThan(4_000, catalog.Subdivisions.Count);
        Assert.IsTrue(report.IsValid);
        Assert.AreEqual(0, report.ErrorCount);
        Assert.IsTrue(catalog.Countries.Any(static country => country.Alpha2 == "US"));
        Assert.IsTrue(catalog.Subdivisions.Any(static subdivision => subdivision.Id == "US-CA"));
    }

    [TestMethod]
    public async Task SearchFindsCountriesByCurrencyAndAlias()
    {
        GeographyCatalog catalog = await new GeographyDatasetLoader().LoadAsync(
            DataDirectory,
            GeographyOverrideSet.Empty);
        GeographySearchIndex index = new(catalog);

        IReadOnlyList<GeographySearchResult> currencyResults = index.Search("JPY");
        IReadOnlyList<GeographySearchResult> aliasResults = index.Search("Vereinigte Staaten");

        Assert.IsTrue(currencyResults.Any(static result => result.Id == "JP"));
        Assert.IsTrue(aliasResults.Any(static result => result.Id == "US"));
    }

    [TestMethod]
    public async Task OverrideStoreRoundTripsAndApplicatorPreservesSourceIdentity()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"atlasops-geography-{Guid.NewGuid():N}");
        string path = Path.Combine(directory, "overrides.json");
        try
        {
            GeographyOverrideSet overrides = new(
                1,
                [
                    new CountryOverride(
                        "CA",
                        "Canada edited",
                        null,
                        null,
                        null,
                        "test",
                        DateTimeOffset.UtcNow),
                ],
                []);
            GeographyOverrideStore store = new(path);
            await store.SaveAsync(overrides);

            GeographyOverrideSet reloaded = await store.LoadAsync();
            GeographyCatalog catalog = await new GeographyDatasetLoader().LoadAsync(
                DataDirectory,
                reloaded);
            Country canada = catalog.Countries.Single(static country => country.Alpha2 == "CA");

            Assert.AreEqual("Canada edited", canada.Name);
            Assert.AreEqual("CAN", canada.Alpha3);
            Assert.AreEqual("test", reloaded.Countries.Single().Notes);
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }
    }

    [TestMethod]
    public void ValidatorDetectsMissingCountryReferences()
    {
        GeographyCatalog catalog = new(
            [],
            [new AdministrativeDivision("ZZ-A", "ZZ", "A", "Area", "Region", "", null, [])],
            GeographyOverrideSet.Empty,
            new ReferenceDataRelease(
                "test",
                "https://example.invalid",
                "MIT",
                "abc",
                "def",
                DateTimeOffset.UtcNow));

        GeographyValidationReport report = new GeographyCatalogValidator().Validate(catalog);

        Assert.AreEqual(1, report.ErrorCount);
        Assert.AreEqual("subdivision-country-reference", report.Issues.Single().Rule);
    }
}