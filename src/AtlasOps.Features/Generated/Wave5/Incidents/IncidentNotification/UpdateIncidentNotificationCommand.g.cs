namespace AtlasOps.Features.Incidents.IncidentNotification;

public sealed record UpdateIncidentNotificationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);