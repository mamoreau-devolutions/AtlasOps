namespace AtlasOps.Features.Identity.IdentityAuditOptimization;

public sealed record UpdateIdentityAuditOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);