namespace AtlasOps.Features.Incidents.StatusUpdate;

public sealed record StatusUpdateChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);