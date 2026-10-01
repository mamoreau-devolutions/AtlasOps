namespace AtlasOps.Features.Api.ApiEndpointProvisioning;

public sealed record UpdateApiEndpointProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);