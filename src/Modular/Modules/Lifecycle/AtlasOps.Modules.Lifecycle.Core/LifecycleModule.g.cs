namespace AtlasOps.Modules.Lifecycle.Core;

using System.Collections.Generic;

public sealed record LifecycleCapabilityDescriptor(string Id, string DisplayName, string Area, string Concern, string Description);
public static class LifecycleModule
{
    public const string Id = "Lifecycle";
    public const string DisplayName = "Software lifecycle";
    public static IReadOnlyList<LifecycleCapabilityDescriptor> Capabilities { get; } = new LifecycleCapabilityDescriptor[]
    {
        new("Lifecycle.ProductCatalog", "Product catalog", "Product", "Catalog", "Coordinates software lifecycle for Product catalog."),
        new("Lifecycle.ProductClassification", "Product classification", "Product", "Classification", "Coordinates software lifecycle for Product classification."),
        new("Lifecycle.ProductForecasting", "Product forecasting", "Product", "Forecasting", "Coordinates software lifecycle for Product forecasting."),
        new("Lifecycle.ProductRisk", "Product risk", "Product", "Risk", "Coordinates software lifecycle for Product risk."),
        new("Lifecycle.ProductUpgrade", "Product upgrade", "Product", "Upgrade", "Coordinates software lifecycle for Product upgrade."),
        new("Lifecycle.ProductReporting", "Product reporting", "Product", "Reporting", "Coordinates software lifecycle for Product reporting."),
        new("Lifecycle.ReleaseCatalog", "Release catalog", "Release", "Catalog", "Coordinates software lifecycle for Release catalog."),
        new("Lifecycle.ReleaseClassification", "Release classification", "Release", "Classification", "Coordinates software lifecycle for Release classification."),
        new("Lifecycle.ReleaseForecasting", "Release forecasting", "Release", "Forecasting", "Coordinates software lifecycle for Release forecasting."),
        new("Lifecycle.ReleaseRisk", "Release risk", "Release", "Risk", "Coordinates software lifecycle for Release risk."),
        new("Lifecycle.ReleaseUpgrade", "Release upgrade", "Release", "Upgrade", "Coordinates software lifecycle for Release upgrade."),
        new("Lifecycle.ReleaseReporting", "Release reporting", "Release", "Reporting", "Coordinates software lifecycle for Release reporting."),
        new("Lifecycle.CycleCatalog", "Cycle catalog", "Cycle", "Catalog", "Coordinates software lifecycle for Cycle catalog."),
        new("Lifecycle.CycleClassification", "Cycle classification", "Cycle", "Classification", "Coordinates software lifecycle for Cycle classification."),
        new("Lifecycle.CycleForecasting", "Cycle forecasting", "Cycle", "Forecasting", "Coordinates software lifecycle for Cycle forecasting."),
        new("Lifecycle.CycleRisk", "Cycle risk", "Cycle", "Risk", "Coordinates software lifecycle for Cycle risk."),
        new("Lifecycle.CycleUpgrade", "Cycle upgrade", "Cycle", "Upgrade", "Coordinates software lifecycle for Cycle upgrade."),
        new("Lifecycle.CycleReporting", "Cycle reporting", "Cycle", "Reporting", "Coordinates software lifecycle for Cycle reporting."),
        new("Lifecycle.VersionCatalog", "Version catalog", "Version", "Catalog", "Coordinates software lifecycle for Version catalog."),
        new("Lifecycle.VersionClassification", "Version classification", "Version", "Classification", "Coordinates software lifecycle for Version classification."),
        new("Lifecycle.VersionForecasting", "Version forecasting", "Version", "Forecasting", "Coordinates software lifecycle for Version forecasting."),
        new("Lifecycle.VersionRisk", "Version risk", "Version", "Risk", "Coordinates software lifecycle for Version risk."),
        new("Lifecycle.VersionUpgrade", "Version upgrade", "Version", "Upgrade", "Coordinates software lifecycle for Version upgrade."),
        new("Lifecycle.VersionReporting", "Version reporting", "Version", "Reporting", "Coordinates software lifecycle for Version reporting."),
        new("Lifecycle.CampaignCatalog", "Campaign catalog", "Campaign", "Catalog", "Coordinates software lifecycle for Campaign catalog."),
        new("Lifecycle.CampaignClassification", "Campaign classification", "Campaign", "Classification", "Coordinates software lifecycle for Campaign classification."),
        new("Lifecycle.CampaignForecasting", "Campaign forecasting", "Campaign", "Forecasting", "Coordinates software lifecycle for Campaign forecasting."),
        new("Lifecycle.CampaignRisk", "Campaign risk", "Campaign", "Risk", "Coordinates software lifecycle for Campaign risk."),
        new("Lifecycle.CampaignUpgrade", "Campaign upgrade", "Campaign", "Upgrade", "Coordinates software lifecycle for Campaign upgrade."),
        new("Lifecycle.CampaignReporting", "Campaign reporting", "Campaign", "Reporting", "Coordinates software lifecycle for Campaign reporting."),
    };
}