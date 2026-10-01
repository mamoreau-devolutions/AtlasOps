namespace AtlasOps.Features.Identity.IdentityAuditGovernance;

public sealed record UpdateIdentityAuditGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);