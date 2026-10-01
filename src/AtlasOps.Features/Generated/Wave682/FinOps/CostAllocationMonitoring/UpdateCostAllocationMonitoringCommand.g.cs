namespace AtlasOps.Features.FinOps.CostAllocationMonitoring;

public sealed record UpdateCostAllocationMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);