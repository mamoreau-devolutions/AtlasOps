namespace AtlasOps.Features.FinOps.SavingsPlanMonitoring;

public sealed record UpdateSavingsPlanMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);