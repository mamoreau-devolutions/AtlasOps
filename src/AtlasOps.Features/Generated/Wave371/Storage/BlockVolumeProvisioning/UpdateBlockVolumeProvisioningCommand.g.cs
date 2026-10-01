namespace AtlasOps.Features.Storage.BlockVolumeProvisioning;

public sealed record UpdateBlockVolumeProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);