namespace AtlasOps.Features.BusinessContinuity.RecoveryRunbookGovernance;

public sealed record UpdateRecoveryRunbookGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);