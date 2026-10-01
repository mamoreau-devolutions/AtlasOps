namespace AtlasOps.Features.BusinessContinuity.RecoveryFailoverGovernance;

public sealed record UpdateRecoveryFailoverGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);