namespace AtlasOps.Features.Security.SecuritySessionRecovery;

public sealed record UpdateSecuritySessionRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);