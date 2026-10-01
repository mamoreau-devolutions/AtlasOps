namespace AtlasOps.Features.Observability.ObservabilityRetentionGovernance;

public sealed record UpdateObservabilityRetentionGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);