namespace AtlasOps.Features.Edge.EdgeDeviceGovernance;

public sealed record UpdateEdgeDeviceGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);