namespace AtlasOps.Features.Identity.IdentityGroupProvisioning;

public sealed record UpdateIdentityGroupProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);