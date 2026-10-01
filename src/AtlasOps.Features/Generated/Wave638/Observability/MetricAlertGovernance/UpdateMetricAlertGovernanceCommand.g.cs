namespace AtlasOps.Features.Observability.MetricAlertGovernance;

public sealed record UpdateMetricAlertGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);