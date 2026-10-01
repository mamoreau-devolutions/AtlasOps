namespace AtlasOps.Features.Security.SecurityExceptionRecovery;

public sealed record UpdateSecurityExceptionRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);