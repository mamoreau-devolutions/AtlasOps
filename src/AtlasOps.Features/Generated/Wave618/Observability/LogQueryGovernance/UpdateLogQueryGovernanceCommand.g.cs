namespace AtlasOps.Features.Observability.LogQueryGovernance;

public sealed record UpdateLogQueryGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);