namespace AtlasOps.Features.FinOps.CostAnomalyMonitoring;

public sealed record UpdateCostAnomalyMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);