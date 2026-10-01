namespace AtlasOps.Features.Security.SecurityBaselineRecovery;

public sealed record UpdateSecurityBaselineRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);