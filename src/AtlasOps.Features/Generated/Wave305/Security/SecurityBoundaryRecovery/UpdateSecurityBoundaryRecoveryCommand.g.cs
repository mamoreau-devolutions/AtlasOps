namespace AtlasOps.Features.Security.SecurityBoundaryRecovery;

public sealed record UpdateSecurityBoundaryRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);