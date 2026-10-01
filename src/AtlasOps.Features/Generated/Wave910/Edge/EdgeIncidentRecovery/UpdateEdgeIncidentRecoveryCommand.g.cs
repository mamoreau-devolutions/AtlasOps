namespace AtlasOps.Features.Edge.EdgeIncidentRecovery;

public sealed record UpdateEdgeIncidentRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);