namespace AtlasOps.Features.Observability.MetricAlertRecovery;

public sealed record UpdateMetricAlertRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);