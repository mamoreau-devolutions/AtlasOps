namespace AtlasOps.Features.Identity.IdentityFactorProvisioning;

public sealed record UpdateIdentityFactorProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);