namespace AtlasOps.Features.BusinessContinuity.RecoveryObjectiveRecovery;

public sealed record UpdateRecoveryObjectiveRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);