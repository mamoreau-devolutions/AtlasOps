namespace AtlasOps.Features.Security.SecurityIdentityProvisioning;

public sealed record UpdateSecurityIdentityProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);