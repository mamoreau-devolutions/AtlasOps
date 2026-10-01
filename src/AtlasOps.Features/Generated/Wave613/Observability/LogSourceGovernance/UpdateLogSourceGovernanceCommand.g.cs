namespace AtlasOps.Features.Observability.LogSourceGovernance;

public sealed record UpdateLogSourceGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);