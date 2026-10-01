namespace AtlasOps.Features.Api.ApiVersionProvisioning;

public sealed record UpdateApiVersionProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);