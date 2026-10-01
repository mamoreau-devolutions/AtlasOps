namespace AtlasOps.Features.Data.DataTransformProvisioning;

public sealed record UpdateDataTransformProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);