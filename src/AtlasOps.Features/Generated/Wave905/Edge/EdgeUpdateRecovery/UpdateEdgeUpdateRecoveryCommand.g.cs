namespace AtlasOps.Features.Edge.EdgeUpdateRecovery;

public sealed record UpdateEdgeUpdateRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);