namespace AtlasOps.Features.Incidents.AlertSuppression;

public sealed record UpdateAlertSuppressionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);