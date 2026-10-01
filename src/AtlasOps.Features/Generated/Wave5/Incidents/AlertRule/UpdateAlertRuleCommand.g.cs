namespace AtlasOps.Features.Incidents.AlertRule;

public sealed record UpdateAlertRuleCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);