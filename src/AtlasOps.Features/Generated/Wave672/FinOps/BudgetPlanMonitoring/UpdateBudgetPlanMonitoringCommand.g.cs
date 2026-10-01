namespace AtlasOps.Features.FinOps.BudgetPlanMonitoring;

public sealed record UpdateBudgetPlanMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);