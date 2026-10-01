namespace AtlasOps.Features.Security.SecurityKeyProvisioning;

public sealed record UpdateSecurityKeyProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);