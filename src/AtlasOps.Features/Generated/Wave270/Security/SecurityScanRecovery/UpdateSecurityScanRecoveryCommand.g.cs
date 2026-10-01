namespace AtlasOps.Features.Security.SecurityScanRecovery;

public sealed record UpdateSecurityScanRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);