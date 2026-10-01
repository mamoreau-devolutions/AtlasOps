namespace AtlasOps.Features.Identity.IdentityProviderProvisioning;

public sealed record UpdateIdentityProviderProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);