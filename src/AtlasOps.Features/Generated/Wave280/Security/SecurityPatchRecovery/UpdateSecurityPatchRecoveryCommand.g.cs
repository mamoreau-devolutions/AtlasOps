namespace AtlasOps.Features.Security.SecurityPatchRecovery;

public sealed record UpdateSecurityPatchRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);