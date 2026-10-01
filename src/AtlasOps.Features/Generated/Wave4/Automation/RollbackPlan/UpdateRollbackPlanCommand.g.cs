namespace AtlasOps.Features.Automation.RollbackPlan;

public sealed record UpdateRollbackPlanCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);