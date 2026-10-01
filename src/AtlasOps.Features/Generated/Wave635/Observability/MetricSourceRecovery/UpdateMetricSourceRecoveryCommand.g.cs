namespace AtlasOps.Features.Observability.MetricSourceRecovery;

public sealed record UpdateMetricSourceRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);