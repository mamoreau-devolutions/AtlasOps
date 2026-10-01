namespace AtlasOps.Features.Incidents.AlertEnrichment;

public sealed record AlertEnrichmentChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);