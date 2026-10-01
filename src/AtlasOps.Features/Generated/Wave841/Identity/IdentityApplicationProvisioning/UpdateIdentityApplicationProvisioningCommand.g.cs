namespace AtlasOps.Features.Identity.IdentityApplicationProvisioning;

public sealed record UpdateIdentityApplicationProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);