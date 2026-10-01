namespace AtlasOps.Features.FinOps.BudgetPlanProvisioning;

public sealed record UpdateBudgetPlanProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);