namespace AtlasOps.Features.Analytics.MetricDefinition;

public sealed record UpdateMetricDefinitionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);