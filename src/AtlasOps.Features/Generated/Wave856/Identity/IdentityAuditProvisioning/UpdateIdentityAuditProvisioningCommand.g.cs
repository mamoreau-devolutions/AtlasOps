namespace AtlasOps.Features.Identity.IdentityAuditProvisioning;

public sealed record UpdateIdentityAuditProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);