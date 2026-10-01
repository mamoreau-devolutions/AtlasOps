namespace AtlasOps.Product.Tests;

using AtlasOps.Product.Localization;

[TestClass]
public sealed class LocalizationCatalogTests
{
    [TestMethod]
    public void Resolve_ExactParentDefaultAndMissingKeyFollowFallbackChain()
    {
        LocalizationCatalog catalog = new(
            new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["fr-CA"] = new Dictionary<string, string> { ["exact"] = "Exact" },
                ["fr"] = new Dictionary<string, string> { ["parent"] = "Parent" },
                ["en"] = new Dictionary<string, string> { ["default"] = "Default" },
            },
            "en");

        LocalizedValue exact = catalog.Resolve("exact", "fr-CA");
        LocalizedValue parent = catalog.Resolve("parent", "fr-CA");
        LocalizedValue fallback = catalog.Resolve("default", "fr-CA");
        LocalizedValue missing = catalog.Resolve("missing", "fr-CA");

        Assert.AreEqual("Exact", exact.Value);
        Assert.AreEqual("fr-CA", exact.SourceCulture);
        Assert.AreEqual("Parent", parent.Value);
        Assert.AreEqual("fr", parent.SourceCulture);
        Assert.AreEqual("Default", fallback.Value);
        Assert.AreEqual("en", fallback.SourceCulture);
        Assert.AreEqual("[missing]", missing.Value);
        Assert.AreEqual(string.Empty, missing.SourceCulture);
    }

    [TestMethod]
    public void Resolve_DefaultCultureInputDoesNotDuplicateFallback()
    {
        LocalizationCatalog catalog = new(
            new Dictionary<string, IReadOnlyDictionary<string, string>>
            {
                ["en"] = new Dictionary<string, string> { ["key"] = "Value" },
            },
            "en");

        LocalizedValue result = catalog.Resolve("key", "en");

        Assert.AreEqual("Value", result.Value);
        Assert.AreEqual("en", result.SourceCulture);
    }

    [TestMethod]
    public void Validate_MissingDefaultReturnsSingleDiagnostic()
    {
        LocalizationCatalog catalog = new(
            new Dictionary<string, IReadOnlyDictionary<string, string>>(),
            "en");

        IReadOnlyList<string> result = catalog.Validate();

        CollectionAssert.AreEqual(new[] { "Default culture 'en' is missing." }, result.ToArray());
    }

    [TestMethod]
    public void Validate_ReportsEveryMissingBaselineKey()
    {
        LocalizationCatalog catalog = new(
            new Dictionary<string, IReadOnlyDictionary<string, string>>
            {
                ["en"] = new Dictionary<string, string> { ["a"] = "A", ["b"] = "B" },
                ["fr"] = new Dictionary<string, string> { ["a"] = "Un" },
            },
            "en");

        IReadOnlyList<string> result = catalog.Validate();

        CollectionAssert.AreEqual(new[] { "Culture 'fr' is missing key 'b'." }, result.ToArray());
    }
}
