namespace AtlasOps.Features.Api.ApiTokenProvisioning;

public sealed record UpdateApiTokenProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);