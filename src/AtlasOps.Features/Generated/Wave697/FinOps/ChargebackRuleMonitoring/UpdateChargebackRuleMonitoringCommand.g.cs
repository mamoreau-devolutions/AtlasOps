namespace AtlasOps.Features.FinOps.ChargebackRuleMonitoring;

public sealed record UpdateChargebackRuleMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);