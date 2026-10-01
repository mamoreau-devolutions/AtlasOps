namespace AtlasOps.Features.Api.ApiHealthProvisioning;

public sealed record UpdateApiHealthProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);