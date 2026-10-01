namespace AtlasOps.Features.Identity.IdentityAuditRecovery;

public sealed record UpdateIdentityAuditRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);