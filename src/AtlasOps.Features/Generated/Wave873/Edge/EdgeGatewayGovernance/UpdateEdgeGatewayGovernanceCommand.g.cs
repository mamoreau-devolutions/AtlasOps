namespace AtlasOps.Features.Edge.EdgeGatewayGovernance;

public sealed record UpdateEdgeGatewayGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);