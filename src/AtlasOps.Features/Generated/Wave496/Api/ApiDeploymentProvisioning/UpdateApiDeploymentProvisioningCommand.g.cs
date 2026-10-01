namespace AtlasOps.Features.Api.ApiDeploymentProvisioning;

public sealed record UpdateApiDeploymentProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);