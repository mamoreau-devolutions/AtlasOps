namespace AtlasOps.Features.Edge.EdgeNetworkRecovery;

public sealed record UpdateEdgeNetworkRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);