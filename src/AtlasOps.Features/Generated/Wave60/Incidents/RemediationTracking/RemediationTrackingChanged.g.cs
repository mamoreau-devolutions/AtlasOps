namespace AtlasOps.Features.Incidents.RemediationTracking;

public sealed record RemediationTrackingChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);