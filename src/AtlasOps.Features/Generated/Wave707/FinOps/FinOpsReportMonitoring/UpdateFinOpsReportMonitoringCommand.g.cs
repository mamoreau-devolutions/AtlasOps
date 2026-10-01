namespace AtlasOps.Features.FinOps.FinOpsReportMonitoring;

public sealed record UpdateFinOpsReportMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);