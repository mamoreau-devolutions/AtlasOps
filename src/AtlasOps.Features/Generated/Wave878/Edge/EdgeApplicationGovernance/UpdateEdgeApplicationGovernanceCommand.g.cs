namespace AtlasOps.Features.Edge.EdgeApplicationGovernance;

public sealed record UpdateEdgeApplicationGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);