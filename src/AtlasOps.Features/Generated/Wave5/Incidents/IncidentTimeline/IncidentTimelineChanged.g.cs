namespace AtlasOps.Features.Incidents.IncidentTimeline;

public sealed record IncidentTimelineChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);