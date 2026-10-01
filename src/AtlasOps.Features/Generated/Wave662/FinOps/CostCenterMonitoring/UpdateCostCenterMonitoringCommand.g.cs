namespace AtlasOps.Features.FinOps.CostCenterMonitoring;

public sealed record UpdateCostCenterMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);