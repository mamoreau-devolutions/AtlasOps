namespace AtlasOps.Features.BusinessContinuity.RecoveryDependencyGovernance;

public sealed record UpdateRecoveryDependencyGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);