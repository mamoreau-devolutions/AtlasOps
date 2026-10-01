namespace AtlasOps.Features.BusinessContinuity.ContinuityPlanGovernance;

public sealed record UpdateContinuityPlanGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);