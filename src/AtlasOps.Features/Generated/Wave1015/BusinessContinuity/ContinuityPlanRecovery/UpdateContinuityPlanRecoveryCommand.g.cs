namespace AtlasOps.Features.BusinessContinuity.ContinuityPlanRecovery;

public sealed record UpdateContinuityPlanRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);