namespace AtlasOps.Features.Data.DataProductProvisioning;

public sealed record UpdateDataProductProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);