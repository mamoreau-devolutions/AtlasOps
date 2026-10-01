namespace AtlasOps.Features.Hardening.RecoveryMode;

public sealed record UpdateRecoveryModeCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);