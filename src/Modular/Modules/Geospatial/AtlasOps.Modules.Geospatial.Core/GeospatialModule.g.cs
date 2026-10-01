namespace AtlasOps.Modules.Geospatial.Core;

using System.Collections.Generic;

public sealed record GeospatialCapabilityDescriptor(string Id, string DisplayName, string Area, string Concern, string Description);
public static class GeospatialModule
{
    public const string Id = "Geospatial";
    public const string DisplayName = "Geospatial operations";
    public static IReadOnlyList<GeospatialCapabilityDescriptor> Capabilities { get; } = new GeospatialCapabilityDescriptor[]
    {
        new("Geospatial.LayerCatalog", "Layer catalog", "Layer", "Catalog", "Coordinates geospatial operations for Layer catalog."),
        new("Geospatial.LayerIndexing", "Layer indexing", "Layer", "Indexing", "Coordinates geospatial operations for Layer indexing."),
        new("Geospatial.LayerIntersection", "Layer intersection", "Layer", "Intersection", "Coordinates geospatial operations for Layer intersection."),
        new("Geospatial.LayerDistance", "Layer distance", "Layer", "Distance", "Coordinates geospatial operations for Layer distance."),
        new("Geospatial.LayerValidation", "Layer validation", "Layer", "Validation", "Coordinates geospatial operations for Layer validation."),
        new("Geospatial.LayerReporting", "Layer reporting", "Layer", "Reporting", "Coordinates geospatial operations for Layer reporting."),
        new("Geospatial.FeatureCatalog", "Feature catalog", "Feature", "Catalog", "Coordinates geospatial operations for Feature catalog."),
        new("Geospatial.FeatureIndexing", "Feature indexing", "Feature", "Indexing", "Coordinates geospatial operations for Feature indexing."),
        new("Geospatial.FeatureIntersection", "Feature intersection", "Feature", "Intersection", "Coordinates geospatial operations for Feature intersection."),
        new("Geospatial.FeatureDistance", "Feature distance", "Feature", "Distance", "Coordinates geospatial operations for Feature distance."),
        new("Geospatial.FeatureValidation", "Feature validation", "Feature", "Validation", "Coordinates geospatial operations for Feature validation."),
        new("Geospatial.FeatureReporting", "Feature reporting", "Feature", "Reporting", "Coordinates geospatial operations for Feature reporting."),
        new("Geospatial.GeometryCatalog", "Geometry catalog", "Geometry", "Catalog", "Coordinates geospatial operations for Geometry catalog."),
        new("Geospatial.GeometryIndexing", "Geometry indexing", "Geometry", "Indexing", "Coordinates geospatial operations for Geometry indexing."),
        new("Geospatial.GeometryIntersection", "Geometry intersection", "Geometry", "Intersection", "Coordinates geospatial operations for Geometry intersection."),
        new("Geospatial.GeometryDistance", "Geometry distance", "Geometry", "Distance", "Coordinates geospatial operations for Geometry distance."),
        new("Geospatial.GeometryValidation", "Geometry validation", "Geometry", "Validation", "Coordinates geospatial operations for Geometry validation."),
        new("Geospatial.GeometryReporting", "Geometry reporting", "Geometry", "Reporting", "Coordinates geospatial operations for Geometry reporting."),
        new("Geospatial.RouteCatalog", "Route catalog", "Route", "Catalog", "Coordinates geospatial operations for Route catalog."),
        new("Geospatial.RouteIndexing", "Route indexing", "Route", "Indexing", "Coordinates geospatial operations for Route indexing."),
        new("Geospatial.RouteIntersection", "Route intersection", "Route", "Intersection", "Coordinates geospatial operations for Route intersection."),
        new("Geospatial.RouteDistance", "Route distance", "Route", "Distance", "Coordinates geospatial operations for Route distance."),
        new("Geospatial.RouteValidation", "Route validation", "Route", "Validation", "Coordinates geospatial operations for Route validation."),
        new("Geospatial.RouteReporting", "Route reporting", "Route", "Reporting", "Coordinates geospatial operations for Route reporting."),
        new("Geospatial.ProximityCatalog", "Proximity catalog", "Proximity", "Catalog", "Coordinates geospatial operations for Proximity catalog."),
        new("Geospatial.ProximityIndexing", "Proximity indexing", "Proximity", "Indexing", "Coordinates geospatial operations for Proximity indexing."),
        new("Geospatial.ProximityIntersection", "Proximity intersection", "Proximity", "Intersection", "Coordinates geospatial operations for Proximity intersection."),
        new("Geospatial.ProximityDistance", "Proximity distance", "Proximity", "Distance", "Coordinates geospatial operations for Proximity distance."),
        new("Geospatial.ProximityValidation", "Proximity validation", "Proximity", "Validation", "Coordinates geospatial operations for Proximity validation."),
        new("Geospatial.ProximityReporting", "Proximity reporting", "Proximity", "Reporting", "Coordinates geospatial operations for Proximity reporting."),
    };
}