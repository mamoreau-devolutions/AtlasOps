namespace AtlasOps.Features.Incidents.IncidentActionItem;

public sealed record IncidentActionItemChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);