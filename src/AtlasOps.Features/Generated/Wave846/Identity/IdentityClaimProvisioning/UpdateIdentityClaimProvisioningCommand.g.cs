namespace AtlasOps.Features.Identity.IdentityClaimProvisioning;

public sealed record UpdateIdentityClaimProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);