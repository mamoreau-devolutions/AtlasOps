namespace AtlasOps.Features.Edge.EdgeGatewayRecovery;

public sealed record UpdateEdgeGatewayRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);