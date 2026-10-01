namespace AtlasOps.Features.Security.SecurityFindingRecovery;

public sealed record UpdateSecurityFindingRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);