namespace AtlasOps.Features.Sync.ChangeVector;

public sealed record ChangeVectorChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);