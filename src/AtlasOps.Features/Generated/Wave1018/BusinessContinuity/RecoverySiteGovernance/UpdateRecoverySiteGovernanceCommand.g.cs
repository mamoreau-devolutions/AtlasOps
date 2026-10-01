namespace AtlasOps.Features.BusinessContinuity.RecoverySiteGovernance;

public sealed record UpdateRecoverySiteGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);