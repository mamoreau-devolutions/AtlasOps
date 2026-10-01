namespace AtlasOps.Features.Inventory.DiscoveryScan;

public sealed record UpdateDiscoveryScanCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);