namespace AtlasOps.Features.Observability.ObservabilitySloGovernance;

public sealed record UpdateObservabilitySloGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);