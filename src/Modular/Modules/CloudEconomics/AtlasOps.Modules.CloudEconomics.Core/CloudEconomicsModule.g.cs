namespace AtlasOps.Modules.CloudEconomics.Core;

using System.Collections.Generic;

public sealed record CloudEconomicsCapabilityDescriptor(string Id, string DisplayName, string Area, string Concern, string Description);
public static class CloudEconomicsModule
{
    public const string Id = "CloudEconomics";
    public const string DisplayName = "Cloud economics";
    public static IReadOnlyList<CloudEconomicsCapabilityDescriptor> Capabilities { get; } = new CloudEconomicsCapabilityDescriptor[]
    {
        new("CloudEconomics.ProviderCatalog", "Provider catalog", "Provider", "Catalog", "Coordinates cloud economics for Provider catalog."),
        new("CloudEconomics.ProviderNormalization", "Provider normalization", "Provider", "Normalization", "Coordinates cloud economics for Provider normalization."),
        new("CloudEconomics.ProviderSizing", "Provider sizing", "Provider", "Sizing", "Coordinates cloud economics for Provider sizing."),
        new("CloudEconomics.ProviderForecasting", "Provider forecasting", "Provider", "Forecasting", "Coordinates cloud economics for Provider forecasting."),
        new("CloudEconomics.ProviderOptimization", "Provider optimization", "Provider", "Optimization", "Coordinates cloud economics for Provider optimization."),
        new("CloudEconomics.ProviderReporting", "Provider reporting", "Provider", "Reporting", "Coordinates cloud economics for Provider reporting."),
        new("CloudEconomics.OfferingCatalog", "Offering catalog", "Offering", "Catalog", "Coordinates cloud economics for Offering catalog."),
        new("CloudEconomics.OfferingNormalization", "Offering normalization", "Offering", "Normalization", "Coordinates cloud economics for Offering normalization."),
        new("CloudEconomics.OfferingSizing", "Offering sizing", "Offering", "Sizing", "Coordinates cloud economics for Offering sizing."),
        new("CloudEconomics.OfferingForecasting", "Offering forecasting", "Offering", "Forecasting", "Coordinates cloud economics for Offering forecasting."),
        new("CloudEconomics.OfferingOptimization", "Offering optimization", "Offering", "Optimization", "Coordinates cloud economics for Offering optimization."),
        new("CloudEconomics.OfferingReporting", "Offering reporting", "Offering", "Reporting", "Coordinates cloud economics for Offering reporting."),
        new("CloudEconomics.RegionCatalog", "Region catalog", "Region", "Catalog", "Coordinates cloud economics for Region catalog."),
        new("CloudEconomics.RegionNormalization", "Region normalization", "Region", "Normalization", "Coordinates cloud economics for Region normalization."),
        new("CloudEconomics.RegionSizing", "Region sizing", "Region", "Sizing", "Coordinates cloud economics for Region sizing."),
        new("CloudEconomics.RegionForecasting", "Region forecasting", "Region", "Forecasting", "Coordinates cloud economics for Region forecasting."),
        new("CloudEconomics.RegionOptimization", "Region optimization", "Region", "Optimization", "Coordinates cloud economics for Region optimization."),
        new("CloudEconomics.RegionReporting", "Region reporting", "Region", "Reporting", "Coordinates cloud economics for Region reporting."),
        new("CloudEconomics.PriceCatalog", "Price catalog", "Price", "Catalog", "Coordinates cloud economics for Price catalog."),
        new("CloudEconomics.PriceNormalization", "Price normalization", "Price", "Normalization", "Coordinates cloud economics for Price normalization."),
        new("CloudEconomics.PriceSizing", "Price sizing", "Price", "Sizing", "Coordinates cloud economics for Price sizing."),
        new("CloudEconomics.PriceForecasting", "Price forecasting", "Price", "Forecasting", "Coordinates cloud economics for Price forecasting."),
        new("CloudEconomics.PriceOptimization", "Price optimization", "Price", "Optimization", "Coordinates cloud economics for Price optimization."),
        new("CloudEconomics.PriceReporting", "Price reporting", "Price", "Reporting", "Coordinates cloud economics for Price reporting."),
        new("CloudEconomics.CommitmentCatalog", "Commitment catalog", "Commitment", "Catalog", "Coordinates cloud economics for Commitment catalog."),
        new("CloudEconomics.CommitmentNormalization", "Commitment normalization", "Commitment", "Normalization", "Coordinates cloud economics for Commitment normalization."),
        new("CloudEconomics.CommitmentSizing", "Commitment sizing", "Commitment", "Sizing", "Coordinates cloud economics for Commitment sizing."),
        new("CloudEconomics.CommitmentForecasting", "Commitment forecasting", "Commitment", "Forecasting", "Coordinates cloud economics for Commitment forecasting."),
        new("CloudEconomics.CommitmentOptimization", "Commitment optimization", "Commitment", "Optimization", "Coordinates cloud economics for Commitment optimization."),
        new("CloudEconomics.CommitmentReporting", "Commitment reporting", "Commitment", "Reporting", "Coordinates cloud economics for Commitment reporting."),
    };
}