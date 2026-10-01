namespace AtlasOps.Features.Analytics.MetricRetention;

public sealed record UpdateMetricRetentionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);