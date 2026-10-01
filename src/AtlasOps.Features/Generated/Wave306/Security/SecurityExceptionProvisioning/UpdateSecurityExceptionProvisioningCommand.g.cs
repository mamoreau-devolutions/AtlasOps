namespace AtlasOps.Features.Security.SecurityExceptionProvisioning;

public sealed record UpdateSecurityExceptionProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);