namespace AtlasOps.Features.Storage.FileShareProvisioning;

public sealed record UpdateFileShareProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);