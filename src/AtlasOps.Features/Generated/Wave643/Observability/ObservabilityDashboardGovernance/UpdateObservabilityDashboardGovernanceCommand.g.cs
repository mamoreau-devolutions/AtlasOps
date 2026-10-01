namespace AtlasOps.Features.Observability.ObservabilityDashboardGovernance;

public sealed record UpdateObservabilityDashboardGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);