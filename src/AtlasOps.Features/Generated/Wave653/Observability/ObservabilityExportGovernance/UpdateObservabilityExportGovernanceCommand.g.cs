namespace AtlasOps.Features.Observability.ObservabilityExportGovernance;

public sealed record UpdateObservabilityExportGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);