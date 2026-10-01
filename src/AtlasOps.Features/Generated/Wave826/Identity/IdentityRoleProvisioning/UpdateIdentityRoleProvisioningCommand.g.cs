namespace AtlasOps.Features.Identity.IdentityRoleProvisioning;

public sealed record UpdateIdentityRoleProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);