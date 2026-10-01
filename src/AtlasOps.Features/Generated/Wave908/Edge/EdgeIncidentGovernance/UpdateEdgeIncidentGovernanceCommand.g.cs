namespace AtlasOps.Features.Edge.EdgeIncidentGovernance;

public sealed record UpdateEdgeIncidentGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);