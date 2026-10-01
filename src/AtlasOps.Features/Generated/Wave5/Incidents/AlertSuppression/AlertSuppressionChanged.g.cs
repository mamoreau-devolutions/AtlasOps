namespace AtlasOps.Features.Incidents.AlertSuppression;

public sealed record AlertSuppressionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);