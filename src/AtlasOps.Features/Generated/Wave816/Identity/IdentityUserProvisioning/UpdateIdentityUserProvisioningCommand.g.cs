namespace AtlasOps.Features.Identity.IdentityUserProvisioning;

public sealed record UpdateIdentityUserProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);