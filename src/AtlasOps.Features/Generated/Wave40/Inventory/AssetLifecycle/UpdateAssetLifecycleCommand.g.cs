namespace AtlasOps.Features.Inventory.AssetLifecycle;

public sealed record UpdateAssetLifecycleCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);