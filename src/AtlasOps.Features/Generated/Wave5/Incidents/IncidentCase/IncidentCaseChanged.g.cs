namespace AtlasOps.Features.Incidents.IncidentCase;

public sealed record IncidentCaseChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);