namespace AtlasOps.Modules.Geography.Core;

using System.Collections.Generic;

public sealed record GeographyCapabilityDescriptor(string Id, string DisplayName, string Area, string Concern, string Description);
public static class GeographyModule
{
    public const string Id = "Geography";
    public const string DisplayName = "Geography intelligence";
    public static IReadOnlyList<GeographyCapabilityDescriptor> Capabilities { get; } = new GeographyCapabilityDescriptor[]
    {
        new("Geography.CountryCatalog", "Country catalog", "Country", "Catalog", "Coordinates geography intelligence for Country catalog."),
        new("Geography.CountryValidation", "Country validation", "Country", "Validation", "Coordinates geography intelligence for Country validation."),
        new("Geography.CountrySearch", "Country search", "Country", "Search", "Coordinates geography intelligence for Country search."),
        new("Geography.CountryOverride", "Country override", "Country", "Override", "Coordinates geography intelligence for Country override."),
        new("Geography.CountryComparison", "Country comparison", "Country", "Comparison", "Coordinates geography intelligence for Country comparison."),
        new("Geography.CountryReporting", "Country reporting", "Country", "Reporting", "Coordinates geography intelligence for Country reporting."),
        new("Geography.DivisionCatalog", "Division catalog", "Division", "Catalog", "Coordinates geography intelligence for Division catalog."),
        new("Geography.DivisionValidation", "Division validation", "Division", "Validation", "Coordinates geography intelligence for Division validation."),
        new("Geography.DivisionSearch", "Division search", "Division", "Search", "Coordinates geography intelligence for Division search."),
        new("Geography.DivisionOverride", "Division override", "Division", "Override", "Coordinates geography intelligence for Division override."),
        new("Geography.DivisionComparison", "Division comparison", "Division", "Comparison", "Coordinates geography intelligence for Division comparison."),
        new("Geography.DivisionReporting", "Division reporting", "Division", "Reporting", "Coordinates geography intelligence for Division reporting."),
        new("Geography.RegionCatalog", "Region catalog", "Region", "Catalog", "Coordinates geography intelligence for Region catalog."),
        new("Geography.RegionValidation", "Region validation", "Region", "Validation", "Coordinates geography intelligence for Region validation."),
        new("Geography.RegionSearch", "Region search", "Region", "Search", "Coordinates geography intelligence for Region search."),
        new("Geography.RegionOverride", "Region override", "Region", "Override", "Coordinates geography intelligence for Region override."),
        new("Geography.RegionComparison", "Region comparison", "Region", "Comparison", "Coordinates geography intelligence for Region comparison."),
        new("Geography.RegionReporting", "Region reporting", "Region", "Reporting", "Coordinates geography intelligence for Region reporting."),
        new("Geography.BoundaryCatalog", "Boundary catalog", "Boundary", "Catalog", "Coordinates geography intelligence for Boundary catalog."),
        new("Geography.BoundaryValidation", "Boundary validation", "Boundary", "Validation", "Coordinates geography intelligence for Boundary validation."),
        new("Geography.BoundarySearch", "Boundary search", "Boundary", "Search", "Coordinates geography intelligence for Boundary search."),
        new("Geography.BoundaryOverride", "Boundary override", "Boundary", "Override", "Coordinates geography intelligence for Boundary override."),
        new("Geography.BoundaryComparison", "Boundary comparison", "Boundary", "Comparison", "Coordinates geography intelligence for Boundary comparison."),
        new("Geography.BoundaryReporting", "Boundary reporting", "Boundary", "Reporting", "Coordinates geography intelligence for Boundary reporting."),
        new("Geography.LocationCatalog", "Location catalog", "Location", "Catalog", "Coordinates geography intelligence for Location catalog."),
        new("Geography.LocationValidation", "Location validation", "Location", "Validation", "Coordinates geography intelligence for Location validation."),
        new("Geography.LocationSearch", "Location search", "Location", "Search", "Coordinates geography intelligence for Location search."),
        new("Geography.LocationOverride", "Location override", "Location", "Override", "Coordinates geography intelligence for Location override."),
        new("Geography.LocationComparison", "Location comparison", "Location", "Comparison", "Coordinates geography intelligence for Location comparison."),
        new("Geography.LocationReporting", "Location reporting", "Location", "Reporting", "Coordinates geography intelligence for Location reporting."),
    };
}