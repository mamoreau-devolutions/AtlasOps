namespace AtlasOps.Features.Edge.EdgeDeviceRecovery;

public sealed record UpdateEdgeDeviceRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);