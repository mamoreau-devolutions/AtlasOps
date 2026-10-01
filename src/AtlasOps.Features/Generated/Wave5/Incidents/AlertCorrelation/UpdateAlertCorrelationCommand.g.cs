namespace AtlasOps.Features.Incidents.AlertCorrelation;

public sealed record UpdateAlertCorrelationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);