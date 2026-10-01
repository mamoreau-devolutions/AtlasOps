namespace AtlasOps.Features.Identity.IdentityLifecycleProvisioning;

public sealed record UpdateIdentityLifecycleProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);