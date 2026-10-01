namespace AtlasOps.Features.BusinessContinuity.RecoveryRunbookRecovery;

public sealed record UpdateRecoveryRunbookRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);