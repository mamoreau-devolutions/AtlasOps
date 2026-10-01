namespace AtlasOps.Features.Api.ApiClientProvisioning;

public sealed record UpdateApiClientProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);