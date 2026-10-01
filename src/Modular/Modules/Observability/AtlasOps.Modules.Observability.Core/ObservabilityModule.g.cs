namespace AtlasOps.Modules.Observability.Core;

using System.Collections.Generic;

public sealed record ObservabilityCapabilityDescriptor(string Id, string DisplayName, string Area, string Concern, string Description);
public static class ObservabilityModule
{
    public const string Id = "Observability";
    public const string DisplayName = "Observability and metrics";
    public static IReadOnlyList<ObservabilityCapabilityDescriptor> Capabilities { get; } = new ObservabilityCapabilityDescriptor[]
    {
        new("Observability.MetricCollection", "Metric collection", "Metric", "Collection", "Coordinates observability and metrics for Metric collection."),
        new("Observability.MetricAggregation", "Metric aggregation", "Metric", "Aggregation", "Coordinates observability and metrics for Metric aggregation."),
        new("Observability.MetricCorrelation", "Metric correlation", "Metric", "Correlation", "Coordinates observability and metrics for Metric correlation."),
        new("Observability.MetricThreshold", "Metric threshold", "Metric", "Threshold", "Coordinates observability and metrics for Metric threshold."),
        new("Observability.MetricRetention", "Metric retention", "Metric", "Retention", "Coordinates observability and metrics for Metric retention."),
        new("Observability.MetricReporting", "Metric reporting", "Metric", "Reporting", "Coordinates observability and metrics for Metric reporting."),
        new("Observability.TraceCollection", "Trace collection", "Trace", "Collection", "Coordinates observability and metrics for Trace collection."),
        new("Observability.TraceAggregation", "Trace aggregation", "Trace", "Aggregation", "Coordinates observability and metrics for Trace aggregation."),
        new("Observability.TraceCorrelation", "Trace correlation", "Trace", "Correlation", "Coordinates observability and metrics for Trace correlation."),
        new("Observability.TraceThreshold", "Trace threshold", "Trace", "Threshold", "Coordinates observability and metrics for Trace threshold."),
        new("Observability.TraceRetention", "Trace retention", "Trace", "Retention", "Coordinates observability and metrics for Trace retention."),
        new("Observability.TraceReporting", "Trace reporting", "Trace", "Reporting", "Coordinates observability and metrics for Trace reporting."),
        new("Observability.LogCollection", "Log collection", "Log", "Collection", "Coordinates observability and metrics for Log collection."),
        new("Observability.LogAggregation", "Log aggregation", "Log", "Aggregation", "Coordinates observability and metrics for Log aggregation."),
        new("Observability.LogCorrelation", "Log correlation", "Log", "Correlation", "Coordinates observability and metrics for Log correlation."),
        new("Observability.LogThreshold", "Log threshold", "Log", "Threshold", "Coordinates observability and metrics for Log threshold."),
        new("Observability.LogRetention", "Log retention", "Log", "Retention", "Coordinates observability and metrics for Log retention."),
        new("Observability.LogReporting", "Log reporting", "Log", "Reporting", "Coordinates observability and metrics for Log reporting."),
        new("Observability.DashboardCollection", "Dashboard collection", "Dashboard", "Collection", "Coordinates observability and metrics for Dashboard collection."),
        new("Observability.DashboardAggregation", "Dashboard aggregation", "Dashboard", "Aggregation", "Coordinates observability and metrics for Dashboard aggregation."),
        new("Observability.DashboardCorrelation", "Dashboard correlation", "Dashboard", "Correlation", "Coordinates observability and metrics for Dashboard correlation."),
        new("Observability.DashboardThreshold", "Dashboard threshold", "Dashboard", "Threshold", "Coordinates observability and metrics for Dashboard threshold."),
        new("Observability.DashboardRetention", "Dashboard retention", "Dashboard", "Retention", "Coordinates observability and metrics for Dashboard retention."),
        new("Observability.DashboardReporting", "Dashboard reporting", "Dashboard", "Reporting", "Coordinates observability and metrics for Dashboard reporting."),
        new("Observability.ObjectiveCollection", "Objective collection", "Objective", "Collection", "Coordinates observability and metrics for Objective collection."),
        new("Observability.ObjectiveAggregation", "Objective aggregation", "Objective", "Aggregation", "Coordinates observability and metrics for Objective aggregation."),
        new("Observability.ObjectiveCorrelation", "Objective correlation", "Objective", "Correlation", "Coordinates observability and metrics for Objective correlation."),
        new("Observability.ObjectiveThreshold", "Objective threshold", "Objective", "Threshold", "Coordinates observability and metrics for Objective threshold."),
        new("Observability.ObjectiveRetention", "Objective retention", "Objective", "Retention", "Coordinates observability and metrics for Objective retention."),
        new("Observability.ObjectiveReporting", "Objective reporting", "Objective", "Reporting", "Coordinates observability and metrics for Objective reporting."),
    };
}