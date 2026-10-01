namespace AtlasOps.Features.Edge.EdgeApplicationRecovery;

public sealed record UpdateEdgeApplicationRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);