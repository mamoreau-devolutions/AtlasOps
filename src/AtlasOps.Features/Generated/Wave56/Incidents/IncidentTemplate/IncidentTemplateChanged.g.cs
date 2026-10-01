namespace AtlasOps.Features.Incidents.IncidentTemplate;

public sealed record IncidentTemplateChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);