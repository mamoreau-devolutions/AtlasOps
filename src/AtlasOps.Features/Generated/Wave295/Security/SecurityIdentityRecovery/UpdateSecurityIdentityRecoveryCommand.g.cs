namespace AtlasOps.Features.Security.SecurityIdentityRecovery;

public sealed record UpdateSecurityIdentityRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);