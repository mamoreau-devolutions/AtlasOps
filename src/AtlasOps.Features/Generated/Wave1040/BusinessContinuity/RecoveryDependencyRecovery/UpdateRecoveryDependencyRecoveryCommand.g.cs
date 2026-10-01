namespace AtlasOps.Features.BusinessContinuity.RecoveryDependencyRecovery;

public sealed record UpdateRecoveryDependencyRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);