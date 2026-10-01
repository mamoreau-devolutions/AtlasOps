namespace AtlasOps.Features.Edge.EdgeUpdateGovernance;

public sealed record UpdateEdgeUpdateGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);