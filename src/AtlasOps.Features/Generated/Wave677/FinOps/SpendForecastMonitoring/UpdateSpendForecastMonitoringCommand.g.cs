namespace AtlasOps.Features.FinOps.SpendForecastMonitoring;

public sealed record UpdateSpendForecastMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);