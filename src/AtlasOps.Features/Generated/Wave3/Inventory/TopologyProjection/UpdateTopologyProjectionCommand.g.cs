namespace AtlasOps.Features.Inventory.TopologyProjection;

public sealed record UpdateTopologyProjectionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);