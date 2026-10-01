namespace AtlasOps.ReferenceData.Tests;

using AtlasOps.Modules.Geography.Core;
using AtlasOps.Modules.Geospatial.Core;
using AtlasOps.Modules.Lifecycle.Core;

[TestClass]
public sealed class ReferenceModuleCatalogTests
{
    [TestMethod]
    public void GeographyCatalog_ExposesThirtyUniqueStableCapabilities()
    {
        AssertCatalog(
            GeographyModule.Id,
            GeographyModule.DisplayName,
            GeographyModule.Capabilities.Select(static item => (item.Id, item.DisplayName, item.Area, item.Concern)).ToArray(),
            "Geography.CountryCatalog",
            "Geography.LocationReporting");
    }

    [TestMethod]
    public void GeospatialCatalog_ExposesThirtyUniqueStableCapabilities()
    {
        AssertCatalog(
            GeospatialModule.Id,
            GeospatialModule.DisplayName,
            GeospatialModule.Capabilities.Select(static item => (item.Id, item.DisplayName, item.Area, item.Concern)).ToArray(),
            "Geospatial.LayerCatalog",
            "Geospatial.ProximityReporting");
    }

    [TestMethod]
    public void LifecycleCatalog_ExposesThirtyUniqueStableCapabilities()
    {
        AssertCatalog(
            LifecycleModule.Id,
            LifecycleModule.DisplayName,
            LifecycleModule.Capabilities.Select(static item => (item.Id, item.DisplayName, item.Area, item.Concern)).ToArray(),
            "Lifecycle.ProductCatalog",
            "Lifecycle.CampaignReporting");
    }

    private static void AssertCatalog(
        string id,
        string displayName,
        IReadOnlyList<(string Id, string DisplayName, string Area, string Concern)> capabilities,
        string firstId,
        string lastId)
    {
        Assert.IsFalse(string.IsNullOrWhiteSpace(id));
        Assert.IsFalse(string.IsNullOrWhiteSpace(displayName));
        Assert.HasCount(30, capabilities);
        Assert.AreEqual(30, capabilities.Select(static item => item.Id).Distinct(StringComparer.Ordinal).Count());
        Assert.AreEqual(firstId, capabilities[0].Id);
        Assert.AreEqual(lastId, capabilities[^1].Id);
        Assert.IsFalse(capabilities.Any(static item =>
            string.IsNullOrWhiteSpace(item.DisplayName)
            || string.IsNullOrWhiteSpace(item.Area)
            || string.IsNullOrWhiteSpace(item.Concern)));
    }
}