namespace AtlasOps.Features.BusinessContinuity.RecoveryObjectiveGovernance;

public sealed record UpdateRecoveryObjectiveGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);