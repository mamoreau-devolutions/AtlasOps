namespace AtlasOps.Features.Identity.IdentitySessionProvisioning;

public sealed record UpdateIdentitySessionProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);