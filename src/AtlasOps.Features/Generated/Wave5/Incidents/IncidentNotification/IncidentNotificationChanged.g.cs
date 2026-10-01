namespace AtlasOps.Features.Incidents.IncidentNotification;

public sealed record IncidentNotificationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);