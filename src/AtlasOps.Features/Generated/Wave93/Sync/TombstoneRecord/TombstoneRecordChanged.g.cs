namespace AtlasOps.Features.Sync.TombstoneRecord;

public sealed record TombstoneRecordChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);