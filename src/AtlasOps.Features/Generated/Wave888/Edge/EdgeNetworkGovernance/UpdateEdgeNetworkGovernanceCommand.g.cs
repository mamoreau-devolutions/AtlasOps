namespace AtlasOps.Features.Edge.EdgeNetworkGovernance;

public sealed record UpdateEdgeNetworkGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);