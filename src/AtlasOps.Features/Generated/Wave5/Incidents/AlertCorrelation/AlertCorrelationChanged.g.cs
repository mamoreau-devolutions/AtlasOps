namespace AtlasOps.Features.Incidents.AlertCorrelation;

public sealed record AlertCorrelationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);