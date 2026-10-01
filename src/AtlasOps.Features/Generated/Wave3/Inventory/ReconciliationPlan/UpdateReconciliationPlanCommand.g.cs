namespace AtlasOps.Features.Inventory.ReconciliationPlan;

public sealed record UpdateReconciliationPlanCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);