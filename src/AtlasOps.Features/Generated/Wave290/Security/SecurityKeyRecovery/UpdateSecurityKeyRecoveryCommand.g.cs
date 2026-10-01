namespace AtlasOps.Features.Security.SecurityKeyRecovery;

public sealed record UpdateSecurityKeyRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);