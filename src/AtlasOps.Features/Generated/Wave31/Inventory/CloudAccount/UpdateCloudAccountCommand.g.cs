namespace AtlasOps.Features.Inventory.CloudAccount;

public sealed record UpdateCloudAccountCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);