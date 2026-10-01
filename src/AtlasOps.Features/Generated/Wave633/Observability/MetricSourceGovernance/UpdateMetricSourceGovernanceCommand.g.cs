namespace AtlasOps.Features.Observability.MetricSourceGovernance;

public sealed record UpdateMetricSourceGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);